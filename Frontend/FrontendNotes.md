# 클라이언트 단 인증

## 블레이저 와즘의 HttpClient 특성.

1. Redirect 를 처리할 수 없음.  
리다이렉트는 브라우저가 처리함.  
HttpClient가 백엔드에 로그인 요청(Post)을 보내면, 그 응답은 HttpClient 가 아니라, 브라우저가 처리하기 때문에, 굳이 앵커 태그를 쓰지 않아도 됨.
[정정]
브라우저는 클라이언트가 리다이렉트를 처리하지 못하도록 막을 뿐 직접 처리하지 않는다.

## Http Redirect

Post 요청을 Redirect 하면, Post 가 됨. 


