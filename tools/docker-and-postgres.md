# Docker Desktop and PostgreSQL

Needed for lesson 12 (Testcontainers, PostgreSQL mode) and lesson 14 (ZAP in Docker).

## Install Docker Desktop

1. Requirements: Windows 10/11 64-bit, **virtualization enabled** (Task Manager → Performance → CPU → "Virtualization: Enabled"; if not, enable it in the BIOS/UEFI, often called Intel VT-x or AMD-V/SVM).
2. Install WSL 2 (Windows Subsystem for Linux), in PowerShell **as Administrator**:
   ```powershell
   wsl --install
   ```
   Restart Windows.
3. Install Docker Desktop:
   ```powershell
   winget install --id Docker.DockerDesktop -e
   ```
   Or https://www.docker.com/products/docker-desktop/. Keep "Use WSL 2" checked.
4. Start **Docker Desktop** from the Start menu. Wait until it says "Engine running". Accept the terms (free for personal use and learning).
5. Check:
   ```powershell
   docker run --rm hello-world
   ```

Docker Desktop must be **running** whenever you use Docker. Testcontainers starts containers through it.

## Run PostgreSQL for the app

From the repo root:

```powershell
docker compose up -d          # start PostgreSQL on port 5433
docker compose ps             # STATUS should say "healthy"
```

Start the API on it, from `backend`:

```powershell
dotnet run --project src/DoseLab.Api --launch-profile http-postgres
```

In Visual Studio or Rider: choose the **http-postgres** launch profile.

Look at the data:

```powershell
docker compose exec db psql -U doselab -c "select * from taper_plans;"
```

(Optional GUI: DBeaver, `winget install --id dbeaver.dbeaver -e`. Host `localhost`, port `5433`, database, user and password `doselab`.)

Stop: `docker compose down`. Stop and **delete all data**: `docker compose down -v`.

## Run the Testcontainers tests

From `backend`:

```powershell
dotnet test --filter "Category=Docker"
```

The first run downloads the `postgres:17-alpine` image (about 100 MB). Run `docker ps` during the tests to see the container. It is removed at the end.

## Common problems

| Problem | Fix |
| --- | --- |
| `Docker is either not running or misconfigured` | Start Docker Desktop. Wait for "Engine running". |
| `WSL 2 installation is incomplete` | `wsl --update` in an Administrator PowerShell, restart |
| Port 5433 already in use | Another PostgreSQL runs. Change `"5433:5432"` in `docker-compose.yml` and the port in `appsettings.json` |
| Company laptop blocks Docker | Ask IT. Docker Desktop needs a licence in large companies; for learning on your own PC it is free. |
| Tests time out on first run | The image is downloading. Run `docker pull postgres:17-alpine` once, then try again. |
