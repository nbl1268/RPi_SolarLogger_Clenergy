package powercom::monthlystats;

# This class calcs monthly stats
# new
# -- set_outstanding_stats (eg those missing from monthly_summary)
# -- Y -- set_stats_for_month
# --------- check if monthly summary row exists
# ----------- Y -- Updates the data in the existing row
# ----------- N -- Inserts the data as a new row
# -- N then end

use strict;
use warnings;
use DateTime;		# need to ensure that the DateTime lib is installed

sub new {
#	print "##powercom::monthlystats - new\n";
    my ( $class, $globals ) = @_;
    
    my $self;
    $self->{globals} = $globals;
    bless $self, $class;
    
    $self->set_outstanding_stats;		#check and set stats for any dates missing from 'monthly_summary' that exist in 'readings'
    
    my $dt   = DateTime->now(time_zone => "local");   # Stores current (local) date and time as datetime object
    my $date = $dt->ymd;   # Retrieves date as a string in 'yyyy-mm' format
    my $month = substr($date,0, 7);
    
    print "##powercom::monthlystats - new ($month)\n";
    $self->set_stats_for_month( $month );	# sets stats for current date
    
    return $self;
}


#NOTE: my $self = shift; is grabbing the sub's first argument passed in at time of call. eg first arg in {globals}
#NOTE: $sth = statement handle

sub set_outstanding_stats {
#	print "##powercom::monthlystats - set_outstanding_stats\n";
    my $self = shift;
    
    my $sth = $self->{globals}->{db}->prepare(
        "select\n"
      . "    substr(reading_datetime, 1, 7) as reading_date\n"
      . "from\n"
      . "    readings left join monthly_summary\n"
      . "        on substr(readings.reading_datetime, 1, 7 ) = monthly_summary.reading_date\n"
      . "where\n"
      . "    monthly_summary.reading_date is null\n"
      . "group by substr(reading_datetime, 1, 7)"
    ) || die( $self->{globals}->{db}->errstr );
    
    $sth->execute()
        || die( $sth->errstr );

#RESULT: if there are records in 'readings' with 'reading_datetime' 
#        value where there is no matching record in the 'Daily_summary' 
#        table then returns that date value; else returns nothing

    while ( my $row = $sth->fetchrow_hashref ) {
#		print "##powercom::monthlystats - call set_stats_for_month\n";
        $self->set_stats_for_month( $row->{reading_date} );
    }
}


sub set_stats_for_month {
# gets the summary of the $month result
# checks if monthly summary row exists
# -- Y -- Updates the data in the existing row
# -- N -- Inserts the data as a new row

#	print "##powercom::monthlystats - set_stats_for_month\n";
    my ( $self, $month ) = @_;
    
	print "##powercom::monthlystats - set_stats_for_month ($month)\n";
    $self->summarise_monthly_sth->execute( $month )
        || die( $self->summarise_monthly_sth->errstr );
    
    my $row = $self->summarise_monthly_sth->fetchrow_hashref;
    
    if ( ! $row ) {
#		print "##powercom::monthlystats - set_stats_for_month ($month) - Didn't get a summary row back for month\n";
        die( "Didn't get a summary row back for month [$month]" );
        
    } else {
        
        if ( $self->check_if_summary_record_exists( $month ) ) {
            
#			print "##powercom::monthlystats - set_stats_for_month ($month) - update_daily_stats_sth\n";
            $self->update_monthly_stats_sth->execute(
                $row->{max_heatsink_temperature}
              , $row->{max_ac_power}
              , $row->{total_accumulated_energy}
              , $month
            ) || die( $self->update_monthly_stats_sth->errstr );
            
        } else {
            
#			print "##powercom::monthlystats - set_stats_for_month ($month) - insert_daily_stats_sth\n";
			$self->insert_monthly_stats_sth->execute(
                $row->{max_heatsink_temperature}
              , $row->{max_ac_power}
              , $row->{total_accumulated_energy}
              , $month
            ) || die( $self->insert_monthly_stats_sth->errstr );
        }
    }
}


sub check_if_summary_record_exists {
# checks if record exists in 'monthly_summary' for date $month; 
# if so returns 1
#	print "##powercom::monthlystats - check_if_summary_record_exists\n";
    my ( $self, $month ) = @_;
    
    if ( ! $self->{check_if_summary_record_exists_sth} ) {
        
        $self->{check_if_summary_record_exists_sth} = $self->{globals}->{db}->prepare(
            "select\n"
          . "    reading_date\n"
          . "from\n"
          . "    monthly_summary\n"
          . "where\n"
          . "    reading_date = ?"
        ) || die( $self->{globals}->{db}->errstr );
        
    }
    
    $self->{check_if_summary_record_exists_sth}->execute( $month )
        || die( $self->{check_if_summary_record_exists_sth}->errstr );
    
    my $row = $self->{check_if_summary_record_exists_sth}->fetchrow_arrayref;
    
    if ( $row ) {
        return 1;
    } else {
        return 0;
    }
}


sub summarise_monthly_sth {
# reviews 'readings' for #month and calc's the max heatsink value, max ac power and total accumulated energy
#	print "##powercom::monthlystats - summarise_monthly_sth\n";
    my ( $self, $month ) = @_;
#    print "#month: $month\n";

    if ( ! $self->{summarise_monthly_sth} ) {
        $self->{summarise_monthly_sth} = $self->{globals}->{db}->prepare(
            "select\n"
          . "    max(heat_sink_temperature) as max_heatsink_temperature\n"
          . "  , max(ac_power) as max_ac_power\n"
          . "  , round(cast(max(accumulated_energy) - min(accumulated_energy) as decimal),2) as total_accumulated_energy\n"
          . "from\n"
          . "    readings\n"
          . "where\n"
          . "    substr(reading_datetime, 1, 7) = ?\n"
          . "group by\n"
          . "    substr(reading_datetime, 1, 7 )"
        ) || die( $self->{globals}->{db}->errstr );
    }
    return $self->{summarise_monthly_sth};
}


sub insert_monthly_stats_sth {
# inserts monthly stats as a new row
#	print "##powercom::monthlystats - insert_monthly_stats_sth\n";
    my $self = shift;
    
    if ( ! $self->{insert_monthly_stats_sth} ) {
        
        $self->{insert_monthly_stats_sth} = $self->{globals}->{db}->prepare(
            "insert into monthly_summary(\n"
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
    return $self->{insert_monthly_stats_sth};
}


sub update_monthly_stats_sth {
# updates monthly stats in an existing row
#	print "##powercom::monthlystats - update_monthly_stats_sth\n";
    my $self = shift;
    if ( ! $self->{update_monthly_stats_sth} ) {
        
        $self->{update_monthly_stats_sth} = $self->{globals}->{db}->prepare(
            "update monthly_summary set\n"
          . "    max_heat_sink_temperature = ?\n"
          . "  , max_ac_power = ?\n"
          . "  , total_ac_power = ?\n"
          . "where\n"
          . "    reading_date = ?"
        ) || die( $self->{globals}->{db}->errstr );
    }
    return $self->{update_monthly_stats_sth};
}

1;
