# 14 — Penetration testing lab

Time: 2.5 hours.

Goal: run a small, real penetration test on this app with OWASP ZAP and Burp Suite, find real issues, and write a pen-test report.

The job ad: "penetration testing capabilities are a plus". Lesson 07 gave you the map (OWASP Top 10, SAST/DAST/SCA). This lesson is hands-on.

## Rules first

- **Only test what you are allowed to test.** Here: your own laptop, `localhost`. Never scan a company or public site without **written** permission and an agreed scope. Unauthorized scanning can be a crime.
- Write the scope before you start: target URLs, in-scope and out-of-scope, test accounts, time window, who to call if something breaks.

## Learn: the pen-test flow

| Phase | You do | Tools |
| --- | --- | --- |
| 1. Scope | Agree targets, rules, time | A one-page scope document |
| 2. Recon | Map every endpoint, parameter, header, technology | Browser F12, Swagger, ZAP spider, Burp proxy history |
| 3. Automated scan | Let a scanner try thousands of known attacks | ZAP baseline / API scan / active scan |
| 4. Manual testing | Business logic, auth, access control, injection with valid data | Burp Repeater, ZAP Requester, `curl` |
| 5. Report | Each finding: severity, evidence, impact, fix | Report template below |
| 6. Retest | Confirm the fixes | Same requests again; add regression tests |

**Severity.** Use CVSS (Common Vulnerability Scoring System) or a simple High / Medium / Low with impact × likelihood. For health data, confidentiality impact is high by default.

## Learn: ZAP and Burp

Both are **intercepting proxies**: the browser sends traffic through them, so you can see, change, and replay every request.

| | OWASP ZAP | Burp Suite Community |
| --- | --- | --- |
| Price | Free, open source | Free (Professional is paid) |
| Automated scanning | Yes: spider, passive and active scan, API scan from OpenAPI | Only in Professional |
| Manual testing | Requester, Fuzzer, breakpoints | **Repeater** (best in class), Intruder (slow in Community), Decoder |
| CI | Docker images: `zap-baseline.py`, `zap-api-scan.py`, `zap-full-scan.py` | Professional / Enterprise only |

Install both: [tools/zap-and-burp.md](../tools/zap-and-burp.md).

### Scan types

- **Baseline scan**: spider + **passive** checks only (headers, cookies, information leaks). Safe. Fast. Good for every pipeline.
- **API scan**: imports the OpenAPI file (`/swagger/v1/swagger.json`) and runs **active** attacks against each operation.
- **Full / active scan**: sends attack payloads. Can change or delete data. Only on test environments.

## In this repo

- The API and its OpenAPI file: http://localhost:5080/swagger/v1/swagger.json (Development only).
- PostgreSQL mode (lesson 12) makes injection testing realistic.
- `.github/workflows/nightly.yml` runs a ZAP API scan in CI and keeps the HTML report as an artifact.

Your mentor may turn on bug 09 for this lesson. Do not check which bugs are on.

## Do

Work in `notes/pentest/`. Start PostgreSQL (`docker compose up -d`), the API with `--launch-profile http-postgres`, and the UI.

### 1. Scope (10 min)

Write `notes/pentest/scope.md`: targets (`http://localhost:5080`, `http://localhost:5173`), in scope (all API endpoints, the UI), out of scope (the database container directly, denial of service), test data (synthetic only), time window.

### 2. Recon (20 min)

1. Open the Swagger page. List every endpoint, method, and parameter in a table.
2. Open the UI with F12 → Network. Save a plan. Which headers go to the server? Which come back?
3. Start Burp, open its built-in browser (Proxy → Open browser), use the app, and read **Proxy → HTTP history**.
4. Answer: which endpoints need a credential? Which return patient data? Which technology and version can you see (headers, error pages)?

### 3. Automated scan (20 min)

Run the ZAP API scan from Docker (a PowerShell line; in Git Bash use `$(pwd)` and `\` for line breaks):

```powershell
mkdir zap-out
docker run --rm -v "${PWD}/zap-out:/zap/wrk:rw" ghcr.io/zaproxy/zaproxy:stable `
  zap-api-scan.py -t http://host.docker.internal:5080/swagger/v1/swagger.json -f openapi -r zap-report.html
```

Open `zap-out/zap-report.html`. For each warning: real risk for this app, or noise? Why?

Then think: the scan sent attack payloads to `POST /api/taper-plans`. Did any of them reach the database? Look at the API console output and at the table (`docker compose exec db psql -U doselab -c "select count(*) from taper_plans;"`). If the count did not change: why not? (Hint: what did ZAP put in `medicationCode`, and which check runs first?)

This is the most important lesson about scanners: **a scanner that cannot produce valid business input never reaches the deep code.**

### 4. Manual testing (60 min)

Use Burp Repeater (or ZAP Requester, or `curl`). Start from a **valid** save request from the proxy history (right-click → Send to Repeater). Change one thing at a time.

| # | Test | What to try |
| --- | --- | --- |
| a | Access control (A01) | Read every `GET` endpoint without any key. What patient data do you get? Read a plan by id. |
| b | Authentication (A07) | Save with no key, an empty key, a wrong key, the key in a different case, the key with spaces around it. |
| c | Brute force (A07) | Is there any limit on wrong keys? Send 50 wrong keys quickly (Burp Intruder, or a loop in Git Bash). Any `429 Too Many Requests`? |
| d | Secrets (A07/A05) | Where else can you find the key? (UI JavaScript bundle; the repo; `appsettings.json`.) What else is in `appsettings.json`? |
| e | Injection (A03) | In `patientName` only, with all other fields valid: `'`, `O'Brien`, `"`, `\`, `' OR '1'='1`, `'; --`. Compare the status codes and bodies. |
| f | Error handling (A05) | If anything returns 500: read the whole response. What does it reveal (stack trace, SQL, file paths, versions)? |
| g | Input limits (A04) | A 1 MB patient name. Doses `-1`, `0`, `1e30`, `"abc"`, `null`. Weeks `2147483648`. Extra unknown fields in the JSON. |
| h | Security headers (A05) | Which of these are missing: `X-Content-Type-Options`, `Content-Security-Policy`, `Strict-Transport-Security`, `Cache-Control: no-store` on patient data? |
| i | CORS (A05) | `curl -i -H "Origin: http://127.0.0.1:9999" http://localhost:5080/api/taper-plans`. Does the answer allow that origin? Which origins are allowed in `Program.cs`? |
| j | Transport (A02) | Is anything served over HTTPS? What does that mean for patient names on a hospital network? |

### 5. Make the scanner find what you found (optional, 20 min)

In ZAP desktop: set the browser to use ZAP as a proxy (or use ZAP's "Manual Explore" browser), save one **valid** plan through the UI, find that request in the History, right-click → **Attack → Active Scan**. Add the `X-Api-Key` header with **Replacer** (Tools → Options → Replacer). Does ZAP now report the injection? Write down what you had to give the scanner.

### 6. Report (30 min)

Write `notes/pentest/report.md`:

```text
# Penetration test report — Dose lab API and UI
Scope and dates: ...
Method: recon, ZAP API scan, manual testing with Burp Repeater
Summary: N findings: X high, Y medium, Z low. The most important: ...

## Finding 1: <title>
Severity: High            OWASP: A03 Injection            CVSS (optional): ...
Where: POST /api/taper-plans, field patientName
Steps: <exact request; curl or raw HTTP>
Evidence: <response status, body excerpt, screenshot>
Impact: <what an attacker can do; patient impact>
Fix: <specific: parameterized queries, global error handler, ...>
Regression test: <the automated test that will guard this>
```

Then turn your top 3 findings into **automated tests** (API tests with `WebApplicationFactory`, or integration tests). A finding without a regression test comes back.

## Check yourself

1. What must you have in writing before a penetration test?
2. Baseline scan vs API scan vs active scan: when is each safe to run?
3. Why did the automated scan miss the injection that a manual request found?
4. Why is a 500 response with a stack trace a finding on its own?
5. What is the difference between a DAST finding and a pen-test finding?
6. Name three findings from this app, with their OWASP category and severity.

## Say it in the interview

- "I'm not a full-time pen tester, but I can run a scoped test: recon with Burp, an automated ZAP API scan, then manual tests with Repeater on valid requests. Scanners miss business-logic and anything behind validation."
- "In my lab, ZAP found only missing headers. Manually, an apostrophe in a patient name gave a 500 with the full SQL in the response: SQL injection plus information disclosure. I reported both and added regression tests."
- "In CI I run a ZAP baseline on every merge and an API scan nightly against the test environment; real pen tests happen before major releases, with written scope."
