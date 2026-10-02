# Loan lab

A small loan-account app for QA automation practice.

The backend is .NET. The UI is React. Data is stored in memory. Restart the API and the saved loans are gone. Two sample loans are loaded again.

You can:

- View the loan list
- Add a loan account
- Preview a DBE repayment schedule
- Save the loan and see it in the list

DBE means equal principal each month. Interest is charged on the remaining balance. The installment gets smaller each month.

## Install the tools

You need Git, the .NET 10 SDK, and Node.js 20 or newer.

Check:

```bash
git --version
dotnet --version
node --version
```

`dotnet --version` must start with `10.`.

Download .NET 10: https://dotnet.microsoft.com/download/dotnet/10.0

Download Node.js: https://nodejs.org

## Start the app

Use two terminals. Leave both open.

### 1. Clone

```bash
git clone https://github.com/ionutpenciuc/loan-lab.git
cd loan-lab
```

### 2. Start the API

```bash
cd backend
dotnet run --project src/LoanLab.Api
```

Wait until the terminal says:

```text
Now listening on: http://localhost:5080
```

The first run can take about a minute. It downloads packages.

API docs: http://localhost:5080/swagger

### 3. Start the UI

Open a second terminal. Go to the same `loan-lab` folder (the folder that contains this README).

```bash
cd frontend
npm install
npm run dev
```

Wait until the terminal shows `http://localhost:5173`.

The first `npm install` can take about a minute.

### 4. Open the app

Open http://localhost:5173

You should see two loans:

- Maria Ionescu
- Andrei Stan

Try the screen once:

1. Click **Add loan account**.
2. Customer name: `Ana Pop`
3. Loan amount: `1200`
4. Annual interest rate: `12`
5. Number of installments: `12`
6. Click **Preview schedule**. Row 1 installment is `112.00`.
7. Click **Save**. `Ana Pop` appears in the list.

## Run the sample tests

Stop is not required. You can leave the app running.

Backend tests, from the `backend` folder:

```bash
dotnet test
```

You should see 3 passed tests.

Browser test, from the `frontend` folder:

```bash
npx playwright install chromium
npm run test:e2e
```

Install Chromium once. Later, run only `npm run test:e2e`.

The browser test starts the API and the UI when they are not already running.

## Next

Read [LEARNING.md](LEARNING.md). It is the 3-day study plan.

## If something fails

| What you see | What to do |
| --- | --- |
| `Now listening` never appears | Run `dotnet --version`. Install the .NET 10 SDK. |
| UI says it could not load loan accounts | The API is not running, or it is not on port 5080. |
| Port already in use | Close the old terminal that still runs the API or the UI. Start it again. |
| `npm` is not found | Install Node.js 20 or newer. Open a new terminal. |
| Playwright cannot find the browser | Run `npx playwright install chromium` again from `frontend`. |
