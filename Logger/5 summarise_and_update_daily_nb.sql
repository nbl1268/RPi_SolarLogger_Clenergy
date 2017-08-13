Update daily_summary 
Set
  max_heat_sink_temperature = 
    (
    select max(heat_sink_temperature)
    from
      readings
    Where
      substr(reading_datetime, 1, 10) = '2016-11-03'
    group by
      substr(reading_datetime, 1, 10)
    )
 ,max_ac_power = 
    (
    select max(ac_power)
    from
      readings
    Where
      substr(reading_datetime, 1, 10) = '2016-11-03'
    group by
      substr(reading_datetime, 1, 10)
    )
 ,total_ac_power =
    (
    select max(accumulated_energy) - min(accumulated_energy)
    from
      readings
    Where
      substr(reading_datetime, 1, 10) = '2016-11-03'
    group by
      substr(reading_datetime, 1, 10)
    )

where
  reading_date = '2016-11-03'