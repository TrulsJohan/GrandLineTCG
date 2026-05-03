# 📄 Documentation & Process Report

## 👥 Group Members

- Truls (Scrum Master)
- Fillip
- Berken

---

## 📌 Project Description

This project is a C# console application that manages tournament and trading conventions. Users can register accounts, create and manage events, browse available events, book tickets, and leave reviews after attending events.

The system demonstrates object-oriented programming principles and core C# features such as collections, LINQ, enums, inheritance, and exception handling.

---

## 🧠 Design Decisions

The application is built using object-oriented design to model real-world entities:

- **Classes** such as `User`, `BaseEvent`, `Booking`, and `Review` represent core entities
- **Inheritance** is used to create different event types (e.g., `Tournament`, `TradingEvent`)
- **Encapsulation** ensures data is protected through properties and access modifiers
- **Enums** are used for fixed values like event category and booking status
- **LINQ** simplifies searching, filtering, and calculating data (e.g., average rating)

This structure makes the application modular, readable, and easy to extend.

---

## ⚙️ Project Plan, Task Breakdown and Responsibilities

| Task | Description | Responsible |
|------|------------|------------|
| Project setup | Create repo, structure solution | All |
| User system | Register, login, profile | Fillip |
| Event system | Create/edit/cancel events | Fillip & Truls |
| Booking system | Book/cancel tickets | Berken |
| Search & filtering | Browse + LINQ queries | Fillip |
| Reviews | Rating + comments | Truls |
| Menu system | Console navigation | All |
| Testing & debugging | Fix bugs, validation | All |
| Documentation | README + report | Truls |

---

## 🧩 Requirements Specification (User Stories)

### 1. As a user, I want to register an account, so that I can use the platform

**Acceptance criteria:**
- Username and password required
- Cannot register duplicate usernames

---

### 2. As a user, I want to log in, so that I can access my account

**Acceptance criteria:**
- Correct credentials required
- Error message on failure

---

### 3. As a user, I want to create an event, so that others can attend

**Acceptance criteria:**
- Must be logged in
- Event must include title, date, and venue

---

### 4. As a user, I want to browse events, so that I can find something to attend

**Acceptance criteria:**
- Only upcoming events shown

---

### 5. As a user, I want to search events, so that I can find specific ones

**Acceptance criteria:**
- Search matches title/description

---

### 6. As a user, I want to book a ticket, so that I can attend an event

**Acceptance criteria:**
- Cannot book own event
- Ticket must be available

---

### 7. As a user, I want to cancel a booking, so that I can free my spot

**Acceptance criteria:**
- Booking status updates
- Ticket becomes available again

---

### 8. As a user, I want to view my bookings, so that I can track my events

**Acceptance criteria:**
- Shows past and upcoming bookings

---

### 9. As a user, I want to leave a review, so that I can rate an event

**Acceptance criteria:**
- Only after event is completed
- Only one review per booking

---

### 10. As a user, I want to see event ratings, so that I can evaluate events

**Acceptance criteria:**
- Average rating is displayed

---

## 🔄 Process Report

### 1. Project Management Philosophy

We followed a Scrum methodology with a project board on GitHub, where tasks were continuously moved between:

- To Do
- In Progress
- Done

This approach worked well because it allowed flexibility and made it easy to track progress.

---

### 2. Development Practices

**Version Control**
- Used Git with feature branches
- Regular commits with descriptive messages

**Task Tracking**
- Used a GitHub Projects board

**Code Review**
- Helped catch bugs early and improve quality

**Communication**
- Used chat (Discord/Messenger) for quick updates
- Occasional meetings to align progress

---

### 3. Reflection

**What went well:**
- Good teamwork and communication
- Clear separation of responsibilities
- Effective use of Git and version control

**Challenges:**
- Some features took longer than expected
- GitHub had downtime in Sprint 2

**What we would improve:**
- Plan tasks more in detail at the start
- Do more frequent code reviews

---

## 🧪 C# Features Used

- **Generic Collections**  
  Used to store users, events, and bookings efficiently

- **LINQ**  
  Used for searching, filtering, and calculating data like ratings

- **Enums**  
  Used for categories, statuses, and types

- **Exception Handling**  
  Used to prevent crashes and handle invalid input

- **Inheritance**  
  Used to model different event types from a base class

---

## ▶️ How to Run

```bash
dotnet restore
dotnet build
dotnet run