# Exam 3 — Mock interview

Time: 50 minutes. Your mentor plays the interviewer.

You may read these questions before. Practice the answers out loud. In the exam, you have no notes. The mentor may ask follow-up questions.

## Questions

1. Tell me about yourself, and why this role. (60 seconds)
2. You join our team. How do you design a test strategy for a medical web app with a .NET backend and a React frontend?
3. Explain the testing pyramid. When would you break it?
4. How do you choose test cases for a rule like "the dose must be from 1 to 20 mg"?
5. How do you test a REST API? What do you check?
6. Our Playwright suite has 400 tests, takes 50 minutes, and 5% fail at random. What do you do?
7. Walk me through a performance test for an API. What do you measure, and what do you report?
8. A performance test fails. How do you find the bottleneck?
9. Design a CI pipeline for our product. What runs on a pull request, on merge, and nightly?
10. How do you use AI agents like Claude Code in QA work? How do you make sure the output is trustworthy? What would you build for our QA community?
11. What is different about testing medical device software?
12. Tell me about a bug you found that mattered. (STAR)
13. You inherit a .NET test suite that is slow and flaky, with mocks everywhere. How do you improve the harness?
14. Have you done penetration testing? Walk me through how you would test our API.
15. Our product has a C++ desktop client. How would you automate its UI, and where does Ranorex fit?
16. Show me an AI workflow you built for testing. How do you know it works?

Possible extra questions:

- How do you test security if you are not a pen tester?
- What is TDD? How does a tester support shift-left?
- `decimal` vs `double` in C#: why does it matter for doses?
- How would you test a C++ desktop application?
- A developer says "it works on my machine". What do you do?
- Where are your gaps, and how are you closing them?

## Scoring

The mentor scores each answer from 1 to 4. See `instructor/answer-keys/exam-3-mock-interview-key.md`.

Pass: average 3 or more, and no score of 1 on questions 2, 3, 6, 7, 9, 10, 13, or 16.
