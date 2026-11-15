FROM python:3.11-slim
WORKDIR /app
COPY mcp_server.py /app/mcp_server.py
CMD ["python", "/app/mcp_server.py"]
