# Article Review Hub #

## Get Started ##

## Usage ##

```bash
# Just open the root directory and run command below
docker compose up

# Or you can run on a supported IDE
```

After the project started successfully, 
navigate to [http://localhost:5000](http://localhost:5000) for the ArticleAPI 
and [http://localhost:5050](http://localhost:5050) for the ReviewAPI

When the project starts, an identity record will be created in ArticleAPI. This is the token request information that will be used by ReviewAPI.

Also, if you want to use swagger then you can use the JSON object below to create a token.

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

