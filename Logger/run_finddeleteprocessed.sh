#!/bin/bash
# find files in folder /home/pi/Desktop/.powercom/processed/
# delete those older than 7 days and less than 300 bytes
#/usr/bin/find /home/pi/Desktop/.powercom/processed/ -type f -mtime +7 -size -300c -exec rm \;
#/usr/bin/find /home/pi/Desktop/.powercom/processed/ -type f -mtime +7 -size -300c -print | sort ;
/usr/bin/find /home/pi/Desktop/.powercom/processed/ -type f -mtime +7 -size -300c -delete;
/usr/bin/find /home/pi/.powercom/processed/ -type f -mtime +7 -size -300c -delete;
