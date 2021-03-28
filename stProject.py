import streamlit as st
import pandas as pd
import numpy as np
import cx_Oracle

st.title('Hazard Board')

conn = cx_Oracle.connect('jpalavec', 'Paje127z', "oracle.cise.ufl.edu:1521/orcl", encoding="UTF-8")
cur = conn.cursor()

stormQuery = """SELECT *
                FROM STORM
                ORDER BY 1 ASC
                    FETCH FIRST 100 ROWS ONLY"""
fatalityQuery = """SELECT *
                    FROM FATALITY
                    ORDER BY 1 ASC
                    FETCH FIRST 100 ROWS ONLY"""
accidentQuery = """SELECT *
                    FROM ACCIDENT
                    ORDER BY 1 ASC
                    FETCH FIRST 100 ROWS ONLY"""
countyQuery = """SELECT *
                    FROM COUNTY
                    ORDER BY 1 ASC
                    FETCH FIRST 100 ROWS ONLY"""

#df_storm = pd.read_sql(stormQuery, con=conn)
#df_fatality = pd.read_sql(fatalityQuery, con=conn)
#df_accident = pd.read_sql(accidentQuery, con=conn)
#df_census = pd.read_sql(censusQuery, con=conn)

DATE_COLUMN = 'date/time'
DATA_URL = ('https://s3-us-west-2.amazonaws.com/streamlit-demo-data/uber-raw-data-sep14.csv.gz')

def load_data(nrows):
    data = pd.read_csv(DATA_URL, nrows=nrows)
    lowercase = lambda x: str(x).lower()
    data.rename(lowercase, axis='columns', inplace=True)
    data[DATE_COLUMN] = pd.to_datetime(data[DATE_COLUMN])
    return data

#create a text element and let the reader know that the data is loading
data_load_state_storm = st.text('loading storm data...')
#load 10,000 rows of data into the dataframe
df_storm = pd.read_sql(stormQuery, con=conn)
#notify the reader that the data was successfully loaded
data_load_state_storm.text('Loading data...done!')
st.subheader('Storm Data')
st.write(df_storm)

#create a text element and let the reader know that the data is loading
data_load_state_fatality = st.text('loading fatality data...')
#load 10,000 rows of data into the dataframe
df_fatality = pd.read_sql(fatalityQuery, con=conn)
#notify the reader that the data was successfully loaded
data_load_state_fatality.text('Loading data...done!')
st.subheader('Fatality Data')
st.write(df_fatality)

#create a text element and let the reader know that the data is loading
data_load_state_accident = st.text('loading accident data...')
#load 10,000 rows of data into the dataframe
df_accident = pd.read_sql(accidentQuery, con=conn)
#notify the reader that the data was successfully loaded
data_load_state_accident.text('Loading data...done!')
st.subheader('Accident Data')
st.write(df_accident)

#create a text element and let the reader know that the data is loading
data_load_state_county = st.text('loading county data...')
#load 10,000 rows of data into the dataframe
df_county = pd.read_sql(countyQuery, con=conn)
#notify the reader that the data was successfully loaded
data_load_state_county.text('Loading data...done!')
st.subheader('County Data')
st.write(df_county)




