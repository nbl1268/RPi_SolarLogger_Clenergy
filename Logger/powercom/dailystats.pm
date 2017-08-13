package powercom::dailystats;

# This class calcs daily stats
# new
# -- set_outstanding_stats (eg those missing from daily_summary)
# -- Y -- set_stats_for_day
# --------- check if daily summary row exists
# ----------- Y -- Updates the data in the existing row
# ----------- N -- Inserts the data as a new row
# -- N then end

use strict;
use warnings;
use DateTime;		# need to ensure that the DateTime lib is installed

sub new {
#	print "##powercom::dailystats - new\n";
    my ( $class, $globals ) = @_;
    
    my $self;
    $self->{globals} = $globals;
    bless $self, $class;
    
    $self->set_outstanding_stats;		#check and set stats for any dates missing from 'daily_summary' that exist in 'readings'
    
    my $dt   = DateTime->now(time_zone => "local");   # Stores current (local) date and time as datetime object
    my $date = $dt->ymd;   # Retrieves date as a string in 'yyyy-mm-dd' format
    
#    print "##powercom::dailystats - new ($dt)\n";
#    print "##powercom::dailystats - new ($date)\n";
    $self->set_stats_for_day( $date );	# sets stats for current date
    
    return $self;
}


#NOTE: my $self = shift; is grabbing the sub's first argument passed in at time of call. eg first arg in {globals}
#NOTE: $sth = statement handle

sub set_outstanding_stats {
#	print "##powercom::dailystats - set_outstanding_stats\n";
    my $self = shift;
    
    my $sth = $self->{globals}->{db}->prepare(
        "select\n"
      . "    substr(reading_datetime, 1, 10) as reading_date\n"
      . "from\n"
      . "    readings left join daily_summary\n"
      . "        on substr(readings.reading_datetime, 1, 10 ) = daily_summary.reading_date\n"
      . "where\n"
      . "    daily_summary.reading_date is null\n"
      . "group by substr(reading_datetime, 1, 10)"
    ) || die( $self->{globals}->{db}->errstr );
    
    $sth->execute()
        || die( $sth->errstr );

#RESULT: if there are records in 'readings' with 'reading_datetime' 
#        value where there is no matching record in the 'Daily_summary' 
#        table then returns that date value; else returns nothing

    while ( my $row = $sth->fetchrow_hashref ) {
#		print "##powercom::dailystats - call set_stats_for_day\n";
        $self->set_stats_for_day( $row->{reading_date} );
    }
}


sub set_stats_for_day {
# gets the summary of the $day result
# checks if daily summary row exists
# -- Y -- Updates the data in the existing row
# -- N -- Inserts the data as a new row

#	print "##powercom::dailystats - set_stats_for_day\n";
    my ( $self, $day ) = @_;
    
#	print "##powercom::dailystats - set_stats_for_day ($day)\n";
    $self->summarise_daily_sth->execute( $day )
        || die( $self->summarise_daily_sth->errstr );
    
    my $row = $self->summarise_daily_sth->fetchrow_hashref;
    
    if ( ! $row ) {
#		print "##powercom::dailystats - set_stats_for_day ($day) - Didn't get a summary row back for day\n";
        die( "Didn't get a summary row back for day [$day]" );
        
    } else {
        
        if ( $self->check_if_summary_record_exists( $day ) ) {
            
#			print "##powercom::dailystats - set_stats_for_day ($day) - update_daily_stats_sth\n";
            $self->update_daily_stats_sth->execute(
                $row->{max_heatsink_temperature}
              , $row->{max_ac_power}
              , $row->{total_accumulated_energy}
              , $day
            ) || die( $self->update_daily_stats_sth->errstr );
            
        } else {
            
#			print "##powercom::dailystats - set_stats_for_day ($day) - insert_daily_stats_sth\n";
			$self->insert_daily_stats_sth->execute(
                $row->{max_heatsink_temperature}
              , $row->{max_ac_power}
              , $row->{total_accumulated_energy}
              , $day
            ) || die( $self->insert_daily_stats_sth->errstr );
        }
    }
}


sub check_if_summary_record_exists {
# checks if record exists in 'daily_summary' for date $day; 
# if so returns 1
#	print "##powercom::dailystats - check_if_summary_record_exists\n";
    my ( $self, $day ) = @_;
    
    if ( ! $self->{check_if_summary_record_exists_sth} ) {
        
        $self->{check_if_summary_record_exists_sth} = $self->{globals}->{db}->prepare(
            "select\n"
          . "    reading_date\n"
          . "from\n"
          . "    daily_summary\n"
          . "where\n"
          . "    reading_date = ?"
        ) || die( $self->{globals}->{db}->errstr );
        
    }
    
    $self->{check_if_summary_record_exists_sth}->execute( $day )
        || die( $self->{check_if_summary_record_exists_sth}->errstr );
    
    my $row = $self->{check_if_summary_record_exists_sth}->fetchrow_arrayref;
    
    if ( $row ) {
        return 1;
    } else {
        return 0;
    }
}


sub summarise_daily_sth {
# reviews 'readings' for #day and calc's the max heatsink value, max ac power and total accumulated energy
#	print "##powercom::dailystats - summarise_daily_sth\n";
    my ( $self, $day ) = @_;
#    print "#day: $day\n";

    if ( ! $self->{summarise_daily_sth} ) {
        $self->{summarise_daily_sth} = $self->{globals}->{db}->prepare(
            "select\n"
          . "    max(heat_sink_temperature) as max_heatsink_temperature\n"
          . "  , max(ac_power) as max_ac_power\n"
          . "  , round(cast(max(accumulated_energy) - min(accumulated_energy) as decimal),2) as total_accumulated_energy\n"
          . "from\n"
          . "    readings\n"
          . "where\n"
          . "    substr(reading_datetime, 1, 10) = ?\n"
          . "group by\n"
          . "    substr(reading_datetime, 1, 10 )"
        ) || die( $self->{globals}->{db}->errstr );
    }
    return $self->{summarise_daily_sth};
}


sub insert_daily_stats_sth {
# inserts daily stats as a new row
#	print "##powercom::dailystats - insert_daily_stats_sth\n";
    my $self = shift;
    
    if ( ! $self->{insert_daily_stats_sth} ) {
        
        $self->{insert_daily_stats_sth} = $self->{globals}->{db}->prepare(
            "insert into daily_summary(\n"
          . "    max_heat_sink_temperature\n"
          . "  , max_ac_power\n"
          . "  , total_ac_power\n"
          . "  , reading_date\n"
          . "  , uploaded\n"
          . ") values (\n"
          . "    ?\n"
          . "  , ?\n"
          . "  , ?\n"
          . "  , ?\n"
          . "  , 0\n"
          . ")"
        ) || die( $self->{globals}->{db}->errstr );
    }
    return $self->{insert_daily_stats_sth};
}


sub update_daily_stats_sth {
# updates daily stats in an existing row
#	print "##powercom::dailystats - update_daily_stats_sth\n";
    my $self = shift;
    if ( ! $self->{update_daily_stats_sth} ) {
        
        $self->{update_daily_stats_sth} = $self->{globals}->{db}->prepare(
            "update daily_summary set\n"
          . "    max_heat_sink_temperature = ?\n"
          . "  , max_ac_power = ?\n"
          . "  , total_ac_power = ?\n"
          . "where\n"
          . "    reading_date = ?"
        ) || die( $self->{globals}->{db}->errstr );
    }
    return $self->{update_daily_stats_sth};
}

1;
