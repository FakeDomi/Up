# Dependencies

## UpClient

- UpCore
- UpCore.Windows
- [DarkControls](https://git.domi.re/Domi/DarkControls)

## UpServer

- UpCore
- [NanoDB](https://git.domi.re/Domi/NanoDB)
- [UpWeb](https://git.domi.re/Domi/UpWeb) (only if the up web app is enabled)

# Notes regarding nginx as reverse proxy

Generally speaking, it is a very good idea to use nginx as a reverse proxy for the up backend server. Benefits of doing that include:

- Easy and painless HTTPS setup
- Running multiple services on the same default ports (80 / 443)
- IPv6 access to the web server (Mono's `HttpListener` doesn't support IPv6)

Please note that nginx' default proxy settings are not ideal for large file transfers. Buffering the whole request in RAM or on disk is not desirable and should therefore be turned off. A simple configuration for an nginx server block could look like this:

```
server {
    listen 443 ssl;
    listen [::]:443 ssl;
    server_name up.your-domain.com;

    client_max_body_size 0;
    proxy_http_version 1.1;
    proxy_buffering off;
    proxy_request_buffering off;

    location / {
        proxy_set_header X-Real-IP $remote_addr;
        proxy_pass http://127.0.0.1:1880;
    }
}
```

Should you decide to use SSL for your service, you need to change the `UrlOverride` property in UpServer's `config.xml` to `https://up.your-domain.com` (replace with your own domain).
