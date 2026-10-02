import { defineConfig, devices } from '@playwright/test'

const apiPort = Number(process.env.LAB_API_PORT ?? 5080)
const uiPort = Number(process.env.LAB_UI_PORT ?? 5173)
const apiUrl = `http://localhost:${apiPort}`
const uiUrl = `http://localhost:${uiPort}`

export default defineConfig({
  testDir: './e2e',
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: 0,
  reporter: process.env.CI ? [['list'], ['html', { open: 'never' }]] : 'list',
  use: {
    baseURL: uiUrl,
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
  webServer: [
    {
      command: `dotnet run --project ../backend/src/DoseLab.Api/DoseLab.Api.csproj --no-launch-profile --urls ${apiUrl}`,
      url: `${apiUrl}/api/medications`,
      reuseExistingServer: !process.env.CI,
      timeout: 180_000,
      env: { ASPNETCORE_ENVIRONMENT: 'Development' },
    },
    {
      command: `npm run dev -- --port ${uiPort}`,
      url: uiUrl,
      reuseExistingServer: !process.env.CI,
      timeout: 120_000,
      env: { VITE_API_BASE: apiUrl },
    },
  ],
})
