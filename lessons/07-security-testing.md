# 07 — Security testing basics

Time: 40 minutes.

Goal: add security checks to the test pyramid, find and report real findings in this app, and talk about penetration testing with confidence.

The job ad says "penetration testing capabilities are a plus". You do not need to be a pen tester. You need to show that you think like an attacker and test for the common risks.

## Learn

### OWASP Top 10 (web, 2021)

| # | Risk | What to try |
| --- | --- | --- |
| A01 | Broken access control | Call endpoints without login, or as another user. Change ids in URLs. |
| A02 | Cryptographic failures | Data sent or stored without encryption. Weak hashes. |
| A03 | Injection (SQL, script / XSS, command) | `' OR 1=1 --`, `<script>alert(1)</script>` in every input |
| A04 | Insecure design | Missing limits, missing rules (no maximum dose) |
| A05 | Security misconfiguration | Debug pages, Swagger, stack traces in production; open CORS |
| A06 | Vulnerable components | Old packages with known CVEs |
| A07 | Identification and authentication failures | Weak passwords, no lockout, keys in code |
| A08 | Software and data integrity failures | Unsigned updates, untrusted pipelines |
| A09 | Logging and monitoring failures | No audit log of who changed a prescription |
| A10 | Server-side request forgery (SSRF) | Server fetches a URL you control |

For APIs, also read the **OWASP API Security Top 10**. Its number one risk is BOLA (Broken Object Level Authorization): user A can read user B's record by changing an id.

### Kinds of security testing

| Kind | What | Tools |
| --- | --- | --- |
| SAST (static) | Read the code for weak patterns | SonarQube, CodeQL, Roslyn analyzers |
| SCA (dependencies) | Known vulnerable packages | `dotnet list package --vulnerable`, `npm audit`, Dependabot, Snyk |
| Secrets scanning | Keys and passwords in the repo | GitHub secret scanning, gitleaks |
| DAST (dynamic) | Attack the running app | OWASP ZAP, Burp Suite |
| Penetration test | A human attacker, with a plan and a report | Burp Suite, ZAP, manual work |

Pen test steps: **scope and rules** (written permission!) → **recon** (map endpoints) → **attack** → **report** (finding, severity, evidence, fix).

### Health data

Patient data is sensitive personal data (GDPR in the EU, HIPAA in the US). Access must be limited, logged, and audited. A leak is a high-severity finding.

## In this repo

- `POST /api/taper-plans` needs the header `X-Api-Key`. See `backend/src/DoseLab.Api/Security/RequireApiKeyAttribute.cs`.
- Other endpoints need nothing.
- React escapes text by default, which blocks most script injection.

## Do

1. **Auth tests at the API level.** You wrote these in lesson 03: no key → 401, wrong key → 401, right key → 201. If not, write them now. They are cheap and run on every push.
2. **Find the access gap.** Which endpoints return patient names with no key at all? Write a finding in `notes/security-findings.md`:
   - Title, endpoint, steps to reproduce (a `curl` command), expected, actual, severity, OWASP category, suggested fix.
3. **Find the secret that is not secret.** Open the UI in the browser, press F12, and look at the JavaScript sources or the request headers for a save. Can you see the API key? Write it as a second finding. (A key in a browser app is public. Real apps use user login, for example OAuth 2.0 / OpenID Connect tokens.)
4. **Script injection test.** In Playwright, save a plan with patient name `<img src=x onerror=alert(1)>`. Assert the cell shows the text as written, and that no dialog opened:
   ```ts
   let dialogOpened = false
   page.on('dialog', (dialog) => { dialogOpened = true; void dialog.dismiss() })
   // ... save the plan, then:
   await expect(page.getByRole('cell', { name: '<img src=x onerror=alert(1)>' })).toBeVisible()
   expect(dialogOpened).toBe(false)
   ```
5. **Long input.** API test: a 10,000-character patient name must give 400, not 500.
6. **Dependencies.** Run `dotnet list package --vulnerable --include-transitive` in `backend`, and `npm audit` in `frontend`. Note the result.
7. **Optional DAST.** If you have Docker, run a ZAP baseline scan on the UI:
   ```bash
   docker run --rm -t ghcr.io/zaproxy/zaproxy:stable zap-baseline.py -t http://host.docker.internal:5173
   ```
   Read 3 warnings. Which are real risks for this app?

## Check yourself

1. Name five OWASP Top 10 risks.
2. SAST vs DAST vs SCA: one sentence each.
3. What is BOLA? How would you test for it?
4. Why is an API key inside a React app not a secret?
5. Where in the pipeline do security checks run?
6. What must you have before you start a penetration test?

## Say it in the interview

- "I build security into the pyramid: auth and input tests at API level on every push, dependency and secret scans in CI, and a ZAP baseline scan nightly."
- "In my practice app I found that patient data was readable without any credential. I reported it as broken access control, high severity, because it is health data."
- "For deeper pen testing I use Burp or ZAP, always with written scope and permission, and I report findings with evidence and a fix."
