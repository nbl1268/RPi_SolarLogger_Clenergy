select
  min(accumulated_energy)
 ,max(accumulated_energy)
 ,round(cast(max(accumulated_energy) - min(accumulated_energy) as decimal),2) as total_accumulated_energy
from
  readings
where
  substr(reading_datetime, 1, 10) = '2016-11-02'
group by
  substr(reading_datetime, 1, 10)