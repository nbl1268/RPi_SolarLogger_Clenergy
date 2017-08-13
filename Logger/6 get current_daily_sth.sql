select
  reading_datetime, ac_power
from
  readings
where
  substr(reading_datetime, 1, 10) = '2016-12-25'