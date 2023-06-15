# Jellyfish-Shop
Example of fully functional web application

### Running project
Issue commands `cd <PROJECT_FOLDER>` and `docker compose -f "docker-compose.yaml" up -d --build`

### Database inspecting
Download MongoDB Compass and use `mongodb://root:root@localhost:27017/` connection string to connect

### Endpoints inspecting
Open Swagger in browser by going to `http://localhost:4000/api/swagger/index.html`

### Endpoints calling
#### Get
- Default - GET http://localhost:4000/api/Jellyfish
- With pagination - GET http://localhost:4000/api/Jellyfish?PageNumber=2&PageSize=3
- With sorting - GET http://localhost:4000/api/Jellyfish?SortProperty=Price&Ascending=False
- With filtering - GET http://localhost:4000/api/Jellyfish?Name=Pink Puff&MaxPrice=15&MinAddDate=2023-02-01T12:00:00Z
- Everything combined - GET http://localhost:4000/api/Jellyfish?MaxPrice=25&MinAddDate=2023-01-18T12:00:00Z&PageNumber=2&PageSize=3&SortProperty=Price&Ascending=False


