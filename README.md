# AdPlatforms
Тестовое задание: https://drive.google.com/file/d/15oQnlsRLQuFS1H0pSOseZnReJ4DAtfJ6/view?usp=drive_link

Для запуска проекта в Docker необходимо запустить файл run-container.bat или выполнить команды:

docker build -t ad-platforms .

docker run -d -p 8080:8080 --name ad-platforms-container ad-platforms

После этого API будет доступно по адресу http://localhost:8080
