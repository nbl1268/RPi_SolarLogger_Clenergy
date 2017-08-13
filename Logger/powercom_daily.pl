#!/usr/bin/perl

use strict;

use powercom::dailystats;

use config_manager;

use DBI;

my $globals = {
    config_dir  => $ENV{ HOME } . "/.powercom"
};

eval {
    $globals->{db} = DBI->connect(
        "dbi:SQLite:dbname=" . $globals->{config_dir} . "/powercom.db",
        "",
        ""
    ) || die $DBI::errstr;
};

$globals->{config_manager} = config_manager->new( $globals );

{
#    $forms->{main} = powercom::viewer->new( $globals );
#	print "#Calling powercom::dailystats\n";
    $globals->{main} = powercom::dailystats->new( $globals );
}
