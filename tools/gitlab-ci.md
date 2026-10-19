# GitLab CI (optional)

The job ad names GitLab CI. `ci/gitlab-ci.example.yml` is the same pipeline as the GitHub Actions files. To see it run:

1. Create a free account on https://gitlab.com.
2. New project → **Import project** → Repository by URL → your fork's GitHub URL. (Or: Run CI/CD for external repository → GitHub.)
3. In the GitLab project, copy the file: `git mv ci/gitlab-ci.example.yml .gitlab-ci.yml`, commit, push to GitLab.
4. Build → **Pipelines**. Open a job to see its log. Download artifacts from the job page.
5. For nightly jobs: Build → **Pipeline schedules** → New schedule.
6. Variables (for `ai-triage` and `zap-api-scan-nightly`): Settings → CI/CD → **Variables**. Mark secrets as **Masked**.

Notes:

- GitLab.com free accounts get a monthly quota of CI minutes. The Windows runner (`desktop-ui-tests`) and Docker-in-Docker use more. Run them once, not on every push.
- New accounts may need to verify with a phone number or card before shared runners work.
- In a merge request, GitLab shows failed tests (from `reports: junit`) and line coverage in the diff (from `coverage_report`).

Docs: https://docs.gitlab.com/ci/
