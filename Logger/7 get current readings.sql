select 
   reading_datetime, heat_sink_temperature, panel_1_voltage, panel_1_dc_voltage, working_hours, line_current, line_voltage, ac_frequency, ac_power, accumulated_energy
from readings 
order by rowid desc limit 10;