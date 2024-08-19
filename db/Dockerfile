FROM mysql:5.7.22
EXPOSE 8081
EXPOSE 8080
EXPOSE 3306
COPY ./Project/db/migrations/ /home/database/
COPY ./Project/db/dataset/ /home/database/
COPY ./Project/ci/init_database.sh/ /docker-entrypoint-initdb.d/init_database.sh