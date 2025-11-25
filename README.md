# Image Conversion Server

Bu proje, ASP.NET Core ile geliştirilmiş bir görsel dönüştürme API uygulamasıdır.  
Frontend uygulaması Angular 20 ile geliştirilmiştir ve `image-conversion-webapp` reposunda yer alır.

## Özellikler
- Görsel dosyalarını farklı formatlara dönüştürme (örn: `.webp`, `.png`)
- Dönüştürme tamamlandığında SignalR ile tek client’a bildirim gönderme
- RabbitMQ ile asenkron iş kuyruğu
- Dönüştürülen görselleri listeleme
- Redis ile cache ve hızlı erişim

## Kurulum
- RabbitMQ çalışır olmalıdır
- Redis bağlantısı `ConnectionStrings:Redis` altında tanımlı olmalı
- API varsayılan olarak `https://localhost:7218` üzerinde çalışır

## Notlar
- SignalR sadece ilgili client’a mesaj gönderir  
- RabbitMQ yük altında bile asenkron dönüşüm sağlar  
- Redis sonuç ve geçici veriler için kullanılır
- API görselleri base64 formatında döner