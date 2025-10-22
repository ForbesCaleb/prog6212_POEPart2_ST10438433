CMCS — Contract Monthly Claim System (PROG6212)

A demo ASP.NET Core MVC app for Independent Contractor monthly claims.
Lecturers submit claims, Coordinators/Managers review & approve, and everyone can track status end-to-end.

Built to meet Part 1 + Part 2 POE requirements: UML/Design, GUI prototype, working submission, review, document upload, status tracking, testing, version control & error handling.

✨ Features

Authentication (Cookie) with demo accounts (role-based UI)

Welcome / About pages

Fancy role-themed Login (/Auth/Login?role=lecturer|pc1|am1)

Lecturer

Submit Claim (date, hours, rate, activity, notes)

Upload supporting document (pdf/docx/xlsx; max 5MB; filename shown)

My Claims: search + filter + live status/progress bar

Programme Coordinator / Academic Manager

ReviewClaims list

Start Review → Approve/Reject with comment (modal)

Claim status transitions & audit note

Dashboard

Role-aware hero + shortcuts

Recent claims table (lecturer)

File storage

Local wwwroot/uploads/{claimId}/filename.ext (can swap for encrypted storage later)

Error handling & toasts

Friendly messages via TempData["Success"] / TempData["Error"]

Unit test example (Claim.CalculateTotalAmount)

🧪 Demo Accounts

Password = same as username (demo only)

Role	Username	Password
Lecturer	lecturer	lecturer
Programme Coordinator	pc1	pc1
Academic Manager	am1	am1
Admin (not used in UI)	admin	admin

Quick links:

/Auth/Login?role=lecturer

/Auth/Login?role=pc1

/Auth/Login?role=am1

🛠️ Tech & Architecture

ASP.NET Core MVC (.NET 7/8)

Bootstrap 5 for UI

Cookie Authentication

In-memory stores for demo data (IUserStore, IClaimStore)

Local file storage via IFileStorage

Project Structure (high-level)
Controllers/
  AuthController.cs        // login/logout, role theming
  HomeController.cs        // Welcome, About, Error
  DashboardController.cs   // Lecturer dashboard, SubmitClaim, MyClaims, status JSON
  ManagementController.cs  // ReviewClaims, StartReview, Approve, Reject

Models/
  LoginViewModel.cs
  Claim.cs                 // domain model: hours, rate, total, status, etc.
  ClaimDto.cs              // projection for UI tables
  Enums.cs                 // ClaimStatus (Submitted/UnderReview/Approved/Rejected)
  Interfaces.cs            // IUserStore, IClaimStore, IFileStorage
  InMemoryUserStore.cs
  InMemoryClaimStore.cs
  LocalFileStorage.cs      // saves uploads under wwwroot/uploads

Views/
  Shared/_Layout.cshtml    // navbar, toasts, @RenderSection("Styles"/"Scripts")
  Home/Index.cshtml        // welcome
  Home/About.cshtml
  Auth/Login.cshtml        // themed & prefilled by role
  Dashboard/Index.cshtml   // role-aware hero + shortcuts + recent claims
  Dashboard/SubmitClaim.cshtml // live total calc + validations + upload
  Dashboard/MyClaims.cshtml    // filter/search + progress bar + live refresh
  Management/ReviewClaims.cshtml // approve/reject modals + start review

wwwroot/
  css/site.css             // light polish + .hero styles
  uploads/                 // stored documents (created at runtime)

🔐 Authentication & Roles

Configured in Program.cs with CookieAuth.

On login, the app creates standard claims:

NameIdentifier, Name, GivenName, Role

Role checks:

Lecturer views available to Lecturer

Management pages guarded by [Authorize(Roles="ProgrammeCoordinator,AcademicManager")]

📄 Claim Flow

Lecturer → Submit Claim

Enter DateWorked, HoursWorked, HourlyRate, Activity, optional Notes

Upload optional supporting document (.pdf .docx .xlsx, size ≤ 5 MB)

Client-side total = Hours × Rate (live)

Server validates & saves via IClaimStore. Status: Submitted.

Management → ReviewClaims

See pending list (Submitted/UnderReview)

Start Review → Status: UnderReview

Approve/Reject (modal with comment)

Status becomes Approved or Rejected

Comment stored (audit)

Lecturer → MyClaims

View claims with Status + progress bar

Filter by status, search by activity

Optional polling to refresh statuses (MyClaimsStatusData endpoint)

📁 File Uploads

Input: <input type="file"> with accept .pdf,.docx,.xlsx

Size limit: 5MB (validated server-side)

Stored using IFileStorage (default LocalFileStorage) to:

wwwroot/uploads/{claimId}/{originalFileName}


File link shown back to user; opens in a new tab.

For Part 2 guidance: you can swap LocalFileStorage for an encrypted storage service later (AES encrypt to disk; decrypt on read).

🧯 Error Handling

Controllers wrap critical ops with try/catch and set TempData["Error"] on failure.

_Layout.cshtml renders Bootstrap toast messages automatically when TempData is set.

Global exception path:

Dev: UseDeveloperExceptionPage()

Prod: UseExceptionHandler("/Home/Error") + HSTS

🧪 Unit Test Example

Claim.CalculateTotalAmount:

[Fact]
public void CalculatedTotalAmount()
{
    var claim = new Claim { Hours = 20, Rate = 670 };
    var getResult = claim.CalculateTotalAmount();
    Assert.Equal(13400, getResult);
}


Add more tests for validations (e.g., negatives rejected, file size validation, etc).

⚙️ Program.cs (what it wires up)

Adds MVC, CookieAuth, Authorization

Registers demo services:

IUserStore → InMemoryUserStore

IClaimStore → InMemoryClaimStore

IFileStorage → LocalFileStorage

Routing:

Default route → Welcome Home/Index

Optional middleware that skips Welcome for authenticated users and sends them to /Dashboard/Index

Static files, HTTPS redirection, HSTS configured

🚀 Getting Started

Clone repo

Open solution in Visual Studio

Ensure .NET 7/8 SDK installed

Run (F5)

Visit:

http://localhost:xxxx/ → Welcome

http://localhost:xxxx/Auth/Login?role=lecturer → lecturer login

🧹 Common Issues & Fixes
“The following sections have been defined… ‘Styles’”

You used @section Styles { ... } in a view, but layout doesn’t render it.
Fix: In Views/Shared/_Layout.cshtml, add in <head>:

@RenderSection("Styles", required: false)


(and keep @RenderSection("Scripts", required: false) near </body>)

Windows copy/move Error 0x80010135 (Path too long)

Enable LongPathsEnabled in registry (Windows 10/11)

Or move solution to a shorter path: C:\Projects\CMCS\

Delete bin/ + obj/ (PowerShell if needed)

Error 0x80004005: Unspecified error

Often permissions or long path; take ownership:

takeown /F "C:\Projects\CMCS" /R /D Y
icacls "C:\Projects\CMCS" /grant "%username%":F /T


Then clean bin/obj/.vs and rebuild.

📝 Marking Rubric Notes (how this project maps)

Documentation (Design + Structure):

Clear MVC structure, services, DI, file storage, routing

UML + PK/FK (Part 1):

Provided in docs/ or report — models reflect entities: Lecturer, Claim, ClaimDocument, etc.

Project Plan (Part 1):

Included milestones: design, GUI prototype, submit form, management review, upload, testing

GUI/UI (Part 1):

Non-functional prototype made functional in Part 2 with Bootstrap polish

Version Control (both parts):

Frequent commits with descriptive messages

Part 2 Functionalities:

Submit claim, review/approve/reject, upload docs, status tracking, unit testing, error handling

🔄 Future Enhancements

Persist data in a database (EF Core)

AES encryption at rest for uploaded documents

Email notifications on status changes

Export claims (CSV/PDF)

Admin reports & dashboards

📜 License

Educational/demo use for PROG6212 POE. Adapt and extend freely for learning.