FROM mcr.microsoft.com/mssql/server:2019-CU13-ubuntu-20.04

WORKDIR /usr/src/app

COPY . /usr/src/app

ENV ACCEPT_EULA Y
ENV SA_PASSWORD MyNotesDb!
ENV MSSQL_PID Express

EXPOSE 1433

CMD /bin/bash ./entrypoint.sh