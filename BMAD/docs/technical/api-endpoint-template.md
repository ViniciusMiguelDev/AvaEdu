# API Endpoint Documentation Template

## Endpoint: [HTTP Method] [Endpoint Path]

**Version:** [API Version]
**Last Updated:** [Date]
**Author:** [Author Name]

### Summary
[Brief description of what this endpoint does]

### Authentication
- **Required:** [Yes/No]
- **Type:** [Bearer Token/API Key/OAuth/etc.]
- **Scope:** [Required permissions/scopes]

### Rate Limiting
- **Limit:** [X requests per Y time period]
- **Headers:** [Rate limit headers returned]

## Request

### HTTP Method
`[GET|POST|PUT|PATCH|DELETE]`

### URL
```
[Base URL]/[endpoint path]
```

### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `param1`  | string | Yes | [Description] |
| `param2`  | integer | No | [Description] |

### Query Parameters
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `limit`   | integer | No | 20 | [Description] |
| `offset`  | integer | No | 0 | [Description] |
| `filter`  | string | No | - | [Description] |

### Headers
| Header | Required | Description |
|--------|----------|-------------|
| `Content-Type` | Yes | Must be `application/json` |
| `Authorization` | Yes | Bearer token |
| `X-Custom-Header` | No | [Custom header description] |

### Request Body
```json
{
  "field1": "string",
  "field2": 123,
  "field3": {
    "nestedField": "value"
  },
  "field4": [
    "array",
    "values"
  ]
}
```

#### Request Schema
| Field | Type | Required | Description | Validation |
|-------|------|----------|-------------|------------|
| `field1` | string | Yes | [Description] | min: 1, max: 100 |
| `field2` | integer | No | [Description] | min: 0 |
| `field3` | object | No | [Description] | - |
| `field3.nestedField` | string | No | [Description] | - |
| `field4` | array | No | [Description] | max items: 10 |

## Response

### Success Response (200 OK)
```json
{
  "data": {
    "id": "12345",
    "name": "Example Item",
    "created_at": "2025-12-01T10:00:00Z",
    "updated_at": "2025-12-01T10:00:00Z"
  },
  "meta": {
    "total": 1,
    "page": 1,
    "per_page": 20
  }
}
```

#### Response Schema
| Field | Type | Description |
|-------|------|-------------|
| `data` | object | Main response data |
| `data.id` | string | Unique identifier |
| `data.name` | string | Item name |
| `data.created_at` | string (ISO 8601) | Creation timestamp |
| `data.updated_at` | string (ISO 8601) | Last update timestamp |
| `meta` | object | Metadata about the response |
| `meta.total` | integer | Total number of items |
| `meta.page` | integer | Current page number |
| `meta.per_page` | integer | Items per page |

### Error Responses

#### 400 Bad Request
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid request data",
    "details": [
      {
        "field": "field1",
        "message": "Field is required"
      }
    ]
  }
}
```

#### 401 Unauthorized
```json
{
  "error": {
    "code": "UNAUTHORIZED",
    "message": "Invalid or missing authentication token"
  }
}
```

#### 403 Forbidden
```json
{
  "error": {
    "code": "FORBIDDEN",
    "message": "Insufficient permissions"
  }
}
```

#### 404 Not Found
```json
{
  "error": {
    "code": "NOT_FOUND",
    "message": "Resource not found"
  }
}
```

#### 429 Too Many Requests
```json
{
  "error": {
    "code": "RATE_LIMIT_EXCEEDED",
    "message": "Rate limit exceeded"
  }
}
```

#### 500 Internal Server Error
```json
{
  "error": {
    "code": "INTERNAL_ERROR",
    "message": "An unexpected error occurred"
  }
}
```

## Examples

### Example 1: Basic Request
```bash
curl -X GET \
  "https://api.example.com/v1/items/12345" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json"
```

**Response:**
```json
{
  "data": {
    "id": "12345",
    "name": "Example Item",
    "created_at": "2025-12-01T10:00:00Z"
  }
}
```

### Example 2: Request with Parameters
```bash
curl -X GET \
  "https://api.example.com/v1/items?limit=10&filter=active" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json"
```

### Example 3: POST Request with Body
```bash
curl -X POST \
  "https://api.example.com/v1/items" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "New Item",
    "category": "example"
  }'
```

## SDK Examples

### JavaScript
```javascript
const response = await fetch('https://api.example.com/v1/items/12345', {
  method: 'GET',
  headers: {
    'Authorization': 'Bearer YOUR_TOKEN',
    'Content-Type': 'application/json'
  }
});

const data = await response.json();
console.log(data);
```

### Python
```python
import requests

url = "https://api.example.com/v1/items/12345"
headers = {
    "Authorization": "Bearer YOUR_TOKEN",
    "Content-Type": "application/json"
}

response = requests.get(url, headers=headers)
data = response.json()
print(data)
```

### PHP
```php
<?php
$curl = curl_init();

curl_setopt_array($curl, array(
  CURLOPT_URL => 'https://api.example.com/v1/items/12345',
  CURLOPT_RETURNTRANSFER => true,
  CURLOPT_HTTPHEADER => array(
    'Authorization: Bearer YOUR_TOKEN',
    'Content-Type: application/json'
  ),
));

$response = curl_exec($curl);
$data = json_decode($response, true);
curl_close($curl);

print_r($data);
?>
```

## Notes

### Performance Considerations
- [Performance notes and recommendations]
- [Caching behavior]
- [Expected response times]

### Security Considerations
- [Security implications]
- [Data sensitivity notes]
- [Access control requirements]

### Deprecation Notice
[If applicable, include deprecation timeline and migration path]

## Related Endpoints
- [GET /related-endpoint] - [Description]
- [POST /another-endpoint] - [Description]

## Changelog
| Version | Date | Changes |
|---------|------|---------|
| 1.0 | [Date] | Initial version |
| 1.1 | [Date] | [Description of changes] |

---

*Template Version: 1.0*
*Last Updated: December 1, 2025*