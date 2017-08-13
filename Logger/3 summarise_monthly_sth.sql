select
  max(heat_sink_temperature) as max_heatsink_temperature
 ,max(ac_power) as max_ac_power
 ,round(cast(max(accumulated_energy) - min(accumulated_energy) as decimal),2) as total_accumulated_energy
from
  readings
where
  substr(reading_datetime, 1, 7) = '2016-11'
group by
  substr(reading_datetime, 1, 7)