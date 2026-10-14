# OWASP ZAP and Burp Suite

Needed for lessons 07 and 14. **Only scan your own `localhost` app**, or systems you have written permission to test.

## OWASP ZAP

Two ways. Use both.

**Desktop app** (for exploring and manual tests):

```powershell
winget install --id ZAP.ZAP -e
```

Or https://www.zaproxy.org/download/ (the Windows installer includes Java). Start ZAP → "No, I do not want to persist this session" → **Manual Explore** → URL `http://localhost:5173` → Launch Browser. Use the app in that browser; ZAP records everything in **History**. Right-click a request → **Open/Resend with Request Editor** to change and replay it.

To send the API key on every request: Tools → Options → **Replacer** → Add: type "Request Header", match `X-Api-Key`, replacement `lab-dev-key`.

**Docker** (for automated scans, same as CI). Docker Desktop must run. In PowerShell, from the repo root:

```powershell
mkdir zap-out
docker run --rm -v "${PWD}/zap-out:/zap/wrk:rw" ghcr.io/zaproxy/zaproxy:stable `
  zap-api-scan.py -t http://host.docker.internal:5080/swagger/v1/swagger.json -f openapi -r zap-report.html
start zap-out/zap-report.html
```

`host.docker.internal` is how a container reaches your PC. Other scripts in the same image: `zap-baseline.py` (passive only, safe), `zap-full-scan.py` (spider + active).

## Burp Suite Community

```powershell
winget install --id PortSwigger.BurpSuite.Community -e
```

Or https://portswigger.net/burp/communitydownload.

1. Start Burp → Temporary project → Use Burp defaults.
2. **Proxy → Intercept → Open browser**. This browser already uses Burp. Turn **Intercept off** so pages load.
3. Use the app at `http://localhost:5173`. Requests appear in **Proxy → HTTP history**.
4. Right-click a request → **Send to Repeater**. In Repeater, change the body or headers and click **Send**. Compare responses side by side.
5. **Intruder** sends many variations of one request (slow in the free edition, enough for learning).

Free learning: PortSwigger **Web Security Academy** (https://portswigger.net/web-security) has free labs for every OWASP topic. Two or three SQL injection and access control labs are great interview preparation.
