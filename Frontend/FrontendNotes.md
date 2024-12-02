# 클라이언트 구현 시 주의 사항.

## 블레이저 와즘의 HttpClient 특성.

1. Redirect 를 처리할 수 없음.  
HttpClient가 보낸 요청에 대한 응답으로 Redirect 를 받은 경우,  
브라우저는 이를 처리하지도 않고, 클라이언트에게 전달하지도 않음.  

## 브라우저의 form 핸들링.

form 태그의 action 속성의 기본값은 "get"

- 기본값   
input 태그 값들을 UrlEncode 하여 query 로 변환


- "post"  
Content type 을 MultiPart 로 설정 후 바디에 삽입.