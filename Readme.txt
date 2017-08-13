Raspberry Pi
Logging of data form Clenergy SPH15 solar inverter, storing data in SQLite3 database and providing visualisation via webpages


Data collection
Readings
	/home/pi/Desktop/PowerComCode/powercom_poll.pl
	(excuted via run_powercom.sh)

	executed each minute using crontab
	queries Clenergy SPH15 via serial port
	results stored in log file
	parses log file and stores data in SQLITE3 db

Daily
	/home/pi/Desktop/PowerComCode/powercom_daily.pl
	(excuted via run_powercom_daily.sh)

	executed every 5 minutes using crontab
	Summarises readings data into daily data
	- Max HeatSink Temp
	- Max AC Power
	- Total AC Power
	etc

Monthly
	/home/pi/Desktop/PowerComCode/powercom_monthly.pl
	(excuted via run_powercom_monthly.sh)

	executed every 5 minutes using crontab
	Summarises readings data into daily data
	- Max HeatSink Temp
	- Max AC Power
	- Total AC Power
	etc



Webpage provides
- Current readings (from inverter) with table of last 10/20/30/60 readings
- Historical values from database
	- Today (mid night to current time)
	- Daily totals (all data)
	- Monthly totals (all data)


Usage
IP:5000
	Test 'I am Hosted on Raspberry Pi' page


IP:5000/solar
	Home Page; current values and table of last 10/20/30/60 readings


IP:5000/solar_day
	chart (bar and line) all data for current day


IP:5000/solar_daily
	chart (bar and line) all daily summary data


IP:5000/solar_monthly
	chart (bar and line) all monthly summary data

