# Article Review Hub #

## Get Started ##

## Usage ##

```bash
# Just open the root directory and run command below
docker compose up

# Or you can run n supported IDE
```

After the project started successfully, 
navigate to [http://localhost:5000](http://localhost:5000) for the ArticleAPI 
and [http://localhost:5050](http://localhost:5050) for the ReviewAPI

```json
{
    "apiKey": "article-api-key",
    "apiSecret": "article-api-secret"
}
```
```bash
curl -X POST 'http://localhost:5000/api/v1/auth' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
    "apiKey": "article-api-key",
    "apiSecret": "article-api-secret"
  }'

```

