# Jellyfish-Shop
Example of fully functional web application

## Development
To run all containers: 
- `cd <PROJECT_FOLDER>` 
- `docker compose -f "docker-compose.yaml" up -d --build`

To run frontend locally (for hot reloading support):
- `cd <PROJECT_FOLDER>/frontend` 
- `npm run dev`

## Database
Download MongoDB Compass and use `mongodb://root:root@localhost:27017/` connection string to connect

## Backend
### Endpoints
Open Swagger in browser by going to `http://localhost:4000/api/swagger/index.html`

### Endpoints calling
#### Get
- Default - GET http://localhost:4000/api/Jellyfish
- With pagination - GET http://localhost:4000/api/Jellyfish?PageNumber=2&PageSize=3
- With sorting - GET http://localhost:4000/api/Jellyfish?SortProperty=Price&Ascending=False
- With filtering - GET http://localhost:4000/api/Jellyfish?Name=Pink Puff&MaxPrice=15&MinAddDate=2023-02-01T12:00:00Z
- Everything combined - GET http://localhost:4000/api/Jellyfish?MaxPrice=25&MinAddDate=2023-01-18T12:00:00Z&PageNumber=2&PageSize=3&SortProperty=Price&Ascending=False

## Frontend

### Enpoints
#### Get static data
- GET http://localhost:3000/static/jellyfish_onomicon_data/PinkPuff.json

#### Must check
- [React in 100 seconds](https://youtu.be/Tn6-PIqc4UM)
- [React Hooks](https://youtu.be/TNhaISOUy6Q)
- [React Pitfalls](https://youtu.be/b0IZo2Aho9Y)
- [Tailwind CSS](https://tailwind-elements.com/learn/te-foundations/tailwind-css/about/)
- [Tailwind CSS Cheat Sheet](https://nerdcave.com/tailwind-cheat-sheet)
- [Tailwind Elements](https://tailwind-elements.com/learn/te-foundations/basics/introduction/)

