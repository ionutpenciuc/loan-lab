# 06 — CI/CD pipelines

Time: 45 minutes.

Goal: read and change a pipeline, and design one that gives fast, trustworthy feedback.

## Learn

- **CI (Continuous Integration).** Every push builds the code and runs the tests. A red build stops the change.
- **CD.** Continuous **Delivery**: every green build can be released with one click. Continuous **Deployment**: it is released automatically.
- **Pipeline.** Stages and jobs that run in order or in parallel.
- **Quality gate.** A rule that blocks a merge: tests green, coverage did not drop, no high-severity security finding, performance threshold passed.
- **Artifact.** A file the pipeline keeps: test reports, Playwright traces, screenshots, the build.

### Order for fast feedback

```text
push ──► build ──► unit tests ──► API tests ──► UI tests ──► perf smoke ──► deploy to test env
          ~1 min     seconds        seconds        minutes       ~1 min
nightly ──► full UI regression, cross-browser ──► load / stress / soak ──► security scan (DAST)
```

Fast and cheap first. If unit tests fail, do not waste 20 minutes on UI tests.

### Make it trustworthy

- No flaky tests in the blocking path. Quarantine them.
- Same versions everywhere (pin tool versions, lock files like `package-lock.json`).
- Clean environment per run (containers; Testcontainers for real databases in tests).
- Keep reports and traces as artifacts so anyone can see why it failed.
- Branch protection: merge only when the pipeline is green.
- Parallelize and shard long suites (`npx playwright test --shard=1/4`).
- Track pipeline time and flaky rate. Slow pipelines get ignored.

### GitHub Actions vs GitLab CI

| Idea | GitHub Actions (`.github/workflows/*.yml`) | GitLab CI (`.gitlab-ci.yml`) |
| --- | --- | --- |
| Unit of work | `jobs:` with `steps:` | jobs, grouped into `stages:` |
| Order | `needs: [backend]` | `stages:` order, or `needs:` |
| Reuse | `uses: actions/setup-dotnet@v4` | `image:`, `include:`, templates |
| Files to keep | `actions/upload-artifact` | `artifacts: paths:` |
| Test report in UI | marketplace actions | `artifacts: reports: junit:` |
| Conditions | `if:`, `on:` | `rules:` |
| Schedule | `on: schedule: - cron:` | pipeline schedules + `rules: - if: $CI_PIPELINE_SOURCE == "schedule"` |
| Runner | `runs-on: ubuntu-latest` | shared or own runners, `tags:` |

The job ad names GitLab CI. Know both.

## In this repo

- `.github/workflows/ci.yml`. Runs on every push and pull request: backend tests, then Playwright and a k6 smoke in parallel. Keeps reports as artifacts.
- `ci/gitlab-ci.example.yml`. The same pipeline for GitLab, for reading. GitHub does not run it.

## Do

1. Open `.github/workflows/ci.yml`. For each job, say what it does and why it is in that order.
2. On GitHub, open your fork → **Actions**. Enable workflows if asked. Push a commit. Watch the run. Open a log. Download the Playwright report artifact.
3. Make it fail on purpose: on a new branch, break one expected value in a test, push, open a pull request. See the red check. Fix it. See it go green.
4. Add a **nightly** job to `ci.yml` that runs your k6 stress test (`on: schedule`). Use the existing `perf-smoke` job as a model.
5. Read `ci/gitlab-ci.example.yml`. Find the stage, the artifacts, and the scheduled job. Compare with the GitHub file.

## Practice bug hunt 2

Run `instructor/bug.sh random 2`. Restart the API and the UI. Run every test you wrote today and yesterday: `dotnet test`, `npm run test:e2e`, and your two k6 scripts (restart the API before each k6 script). For each failure, note the layer, and write a one-line bug title. Then `instructor/bug.sh list` and `instructor/bug.sh reset`. Did each bug fail at the lowest layer that could see it?

## Check yourself

1. What is a quality gate? Give two examples.
2. Why run unit tests before UI tests?
3. Where do long performance and soak tests run, and why not on every push?
4. What is an artifact? Which ones do you keep for a failed UI test?
5. GitLab: what are `stages`, `artifacts`, and `rules`?

## Say it in the interview

- "My pipeline goes from cheap to expensive: unit, API, UI, then a performance smoke. Long load and soak tests run nightly."
- "The blocking path must be trustworthy. Flaky tests go to quarantine, and I track the flaky rate."
- "Every failure leaves evidence: test reports, traces, screenshots as artifacts."
