from openjdk:20-jdk


ENV MINECRAFT_VERSION=1.18.1

ADD https://serverjars.com/api/fetchJar/vanilla/${MINECRAFT_VERSION} server.jar
RUN echo eula=true > eula.txt
EXPOSE 25565

CMD ["java", "-jar", "server.jar", "-nogui"]