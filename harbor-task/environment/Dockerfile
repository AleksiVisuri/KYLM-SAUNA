FROM ubuntu:22.04

ENV DEBIAN_FRONTEND=noninteractive
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates libx11-6 libxcursor1 libxrandr2 libxinerama1 libxi6 \
    libglu1-mesa libglib2.0-0 libnss3 libsndio7.0 && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app
# Copy the exported headless Unity build here once the training environment is finalized.
# Expected entry point: /app/sauna.x86_64
COPY build/ /app/
RUN chmod +x /app/sauna.x86_64 2>/dev/null || true

ENTRYPOINT ["/app/sauna.x86_64"]
