# Introduction

This project is intended for job applications management, CV and cover letter uploading. It allows applicants to apply for a job, upload CV, cover letter and specify their names and relevant data. All documents can be searched by fields, by phrase queries, boolean queries and geolocations. All documents are preprocessed with Serbian Analyzer.

## Technologies

- Angular 11.2.3.
- Node 14.6.0
- .NET Core 5.0
- Elastic search 7.4.0
- SQL server

## How to run this?

- Run Elastic search 7.4.0 locally
- Install Serbian analyzer plugin **serbian-analyzer-1.0-SNAPSHOT.zip** that is located in **Udd.api** folder.
- Run **Udd.Api** project migrations to create database
- Run **Udd.api** project
- Populate database with Serbian cities by running  below command and specifying path to **rs.csv** file that is located in **Udd.api** ``` curl -X POST "https://localhost:44327/api/cities/<path_to_rs.csv>" -H  "accept: */*" -d "" ```
- Run angular app with ```ng serve```



## Author
Predrag Glavas

## License
[MIT](https://choosealicense.com/licenses/mit/)
