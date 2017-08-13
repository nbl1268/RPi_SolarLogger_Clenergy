select
  substr(reading_datetime, 1, 10) as reading_date
from
  readings left join daily_summary
  on substr(readings.reading_datetime, 1, 10) = daily_summary.reading_date
where
  daily_summary.reading_date is null
group by substr(reading_datetime, 1, 10)