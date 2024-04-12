
import json
import requests
import jmespath
from parsel import Selector
import os
import bs4
import unidecode
import re
from datetime import datetime, timedelta

def SaveJSON(fileName,JsonComic):
  print(fileName+"\n")
  with open(f"{fileName}.json", "w") as outfile:
      json.dump(JsonComic, outfile,indent= 2)

def CreateSlug(accented_string:str):

  # accented_string is of type 'unicode'
  unaccented_string = unidecode.unidecode(accented_string).lower()
  unaccented_string = re.sub(r"[^\w\s]", '', unaccented_string)
  unaccented_string=re.sub(r"\s+","-",unaccented_string)
  return unaccented_string



def convert_string_time_to_datetime(time):
    # Extract the number of days from the string
    if("ngày" in time):
        value = int(time.split()[0])
        return datetime.now() - timedelta(days=value)
    
    if("giờ" in time):
        value = int(time.split()[0])
        return datetime.now() - timedelta(hours=value)
    if("phút" in time):
        value = int(time.split()[0])
        return datetime.now() - timedelta(minutes=value)
    try:
        datetime_string = f"{time}/{datetime.now().year%2000}"
        datetime_format = "%H:%M %d/%m/%y"
        return datetime.strptime(datetime_string, datetime_format)

    except:
        date_format = "%d/%m/%y"
        return datetime.strptime(time, date_format)
 
     