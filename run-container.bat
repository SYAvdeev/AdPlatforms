docker build -t ad-platforms .
docker run -d -p 8080:8080 --name ad-platforms-container ad-platforms
