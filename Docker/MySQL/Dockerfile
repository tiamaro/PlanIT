FROM mysql:8.0.33

ENV MYSQL_ROOT_PASSWORD=gokstad


COPY PlanIT-DBPrivileges.sql /docker-entrypoint-initdb.d/
COPY PlantIT-DB.sql /docker-entrypoint-initdb.d/


EXPOSE 3306
