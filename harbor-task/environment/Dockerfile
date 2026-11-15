FROM ubuntu:22.04

ENV DEBIAN_FRONTEND=noninteractive
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates libx11-6 libxcursor1 libxrandr2 libxinerama1 libxi6 \
    libglu1-mesa libglib2.0-0 libnss3 libsndio7.0 && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY build.x86_64 /app/build.x86_64
COPY UnityPlayer.so /app/UnityPlayer.so
COPY build_Data /app/build_Data
RUN chmod +x /app/build.x86_64

ENTRYPOINT ["/app/build.x86_64", "-batchmode", "-nographics", "-logFile", "-"]
