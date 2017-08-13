/* correct column name in readings table */

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;

ALTER TABLE readings RENAME TO readings_bckup;

CREATE TABLE readings 
( id                       integer primary key autoincrement,
  serial_number            number,
  reading_datetime         datetime,
  heat_sink_temperature    number,
  panel_1_voltage          number,
  panel_1_current          number,
  working_hours            number,
  operating_mode           text,
  tmp_f_value              number,
  pv_1_f_value             number,
  gfci_f_value             number,
  fault_code_high          number,
  fault_code_low           number,
  line_current             number,
  line_voltage             number,
  ac_frequency             number,
  ac_power                 number,
  zac                      number,
  accumulated_energy       number,
  gfci_f_value_volts       number,
  gfci_f_value_hz          number,
  gz_f_value_ohm           number
);

INSERT INTO readings ( serial_number, reading_datetime, heat_sink_temperature, panel_1_voltage, panel_1_current, working_hours, operating_mode, tmp_f_value, pv_1_f_value, gfci_f_value, fault_code_high, fault_code_low, line_current, line_voltage, ac_frequency, ac_power, zac, accumulated_energy, gfci_f_value_volts, gfci_f_value_hz, gz_f_value_ohm )
  SELECT serial_number, reading_datetime, heat_sink_temperature, panel_1_voltage, panel_1_dc_voltage, working_hours, operating_mode, tmp_f_value, pv_1_f_value, gfci_f_value, fault_code_high, fault_code_low, line_current, line_voltage, ac_frequency, ac_power, zac, accumulated_energy, gfci_f_value_volts, gfci_f_value_hz, gz_f_value_ohm
  FROM readings_bckup;

/* DROP TABLE readings_bckup; */

COMMIT;
PRAGMA foreign_keys=on;