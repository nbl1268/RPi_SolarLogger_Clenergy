#!/usr/bin/env python
# test web for powercomdata

#import sqlite3
import sys
#import cgi
import cgitb

# global variables
#speriod=(15*60)-1
#dbname='/home/pi/Desktop/.powercom/powercom.db'


# print the HTTP header
def printHTTPheader():
    print "Content-type: text/html\n\n"


# print the HTML head section
# arguments are the page title and the table for the chart
def printHTMLHead(title, table):
    print "<head>"
    print "    <title>"
    print title
    print "    </title>"
    
#    print_graph_script(table)

    print "</head>"


# get data from the database
# if an interval is passed, 
# return a list of records from the database
def get_data(interval):

    conn=sqlite3.connect(dbname)
    curs=conn.cursor()

    if interval == None:
#        curs.execute("SELECT * FROM temps")
        curs.execute("SELECT * FROM readings")
    else:
#        curs.execute("SELECT * FROM temps WHERE timestamp>datetime('now','-%s hours')" % interval)
#        curs.execute("SELECT * FROM temps WHERE timestamp>datetime('2013-09-19 21:30:02','-%s hours') AND timestamp<=datetime('2013-09-19 21:31:02')" % interval)
        curs.execute("SELECT reading_datetime, heat_sink_temperature FROM readings WHERE reading_datetime>datetime('now','-%s hours')" % interval)

    rows=curs.fetchall()
    conn.close()
    return rows


# convert rows from database into a javascript table
def create_table(rows):
    chart_table=""

    for row in rows[:-1]:
        rowstr="['{0}', {1}],\n".format(str(row[0]),str(row[1]))
        chart_table+=rowstr

    row=rows[-1]
    rowstr="['{0}', {1}]\n".format(str(row[0]),str(row[1]))
    chart_table+=rowstr

    return chart_table


# print the javascript to generate the chart
# pass the table generated from the database info
# https://google-developers.appspot.com/chart/interactive/docs/gallery/linechart
# https://google-developers.appspot.com/chart/interactive/docs/gallery/barchart
# https://developers.google.com/chart/interactive/docs/gallery/columnchart
def print_graph_script(table):

    # google chart snippet
    chart_code="""
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">
      google.load("visualization", "1", {packages:["corechart"]});
      google.setOnLoadCallback(drawChart);
      function drawChart() {
        var data = google.visualization.arrayToDataTable([
          ['Time', 'Temperature'],%s
        ]);

        var options = {
          title: 'Power (kW)'
        };

        var chart = new google.visualization.LineChart(document.getElementById('chart_div'));
        chart.draw(data, options);
      }
    </script>"""

    print chart_code % (table)


# print the div that contains the graph
def show_graph():
    print "<h2>Power (kW) Chart</h2>"
    print '<div id="chart_div" style="width: 900px; height: 500px;"></div>'





#MAIN FUNCTION
# This is where the program starts 
from flask import Flask
app2 = Flask(__name__)

@app2.route('/')
def main():

    cgitb.enable()

    # print the HTTP header
    printHTTPheader()

#    if len(records) != 0:
#        # convert the data into a table
#        table=create_table(records)
#    else:
#        print "No data found"
#    return

    # start printing the page
    print "<html>"
    # print the head section including the table
    # used by the javascript for the chart
#    printHTMLHead("RPi Solar Inverter Logger", table)

    # print the page body
    print "<body>"
    print "<h1>RPi Solar Inverter Logger</h1>"
    print "<hr>"
#    print_time_selector(option)
#    show_graph()
#    show_stats(option)
    print "</body>"
    print "</html>"

    sys.stdout.flush()

if __name__=="__main__":
#    main()
    app2.run(debug=True, host='0.0.0.0')
#    app2.run(host='0.0.0.0')


