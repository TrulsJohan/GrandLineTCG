## Sprint 3 – Final Development & Completion

### Overview

Sprint 3 focused on completing the remaining functional requirements, refining existing features, and preparing the application for final submission.

This sprint included both implementation of missing features and a full review of the assignment requirements to ensure everything was correctly fulfilled.

---

### Features Implemented

**Booking System Improvements (Berken):**
- Multiple booking/ticket types per event (e.g., Standard, VIP)
- Each ticket type includes:
    - Name
    - Price
    - Limited quantity
- Automatic handling of unavailable tickets when capacity is reached
- Bookings store:
    - Event reference
    - Ticket type
    - Price at time of booking
    - Booking date
- Users can cancel bookings, returning tickets to availability
- Cancelled events remain visible to users who had bookings

---

**Event System Enhancements (Fillip):**
- Implemented **multiple event types using inheritance**
- Each event type includes at least one unique field
- Added filtering functionality:
    - By event category
    - By event type

Event types implemented:
- **TournamentEvent** – standard competitive tournaments
- **TradingEvent** – events focused on buying, selling, and trading cards between players

> Note:  
> Initially, we misunderstood the requirement and only implemented one event type.  
> This was later corrected by introducing the **TradingEvent** type to meet the assignment criteria.

---

**Search & UI Improvements (Truls):**
- Implemented keyword-based search:
    - Matches title, description, and venue
- Improved console UI for consistency and usability
- Ensured smoother navigation between menus

---

**Project & Documentation Tasks (Truls):**
- Added remaining tasks to GitHub project board
- Completed Sprint 2 and Sprint 3 documentation
- Wrote final README.md
- Conducted overall team reflection

---

### Challenges

During this sprint, we encountered a key issue:

- We initially misunderstood part of the assignment requirements, specifically the need for **at least two distinct event types**
- This required refactoring parts of the codebase to introduce proper inheritance and additional event types

Additionally:
- Some smaller missing features were discovered late in development
- These were addressed by reviewing the assignment requirements carefully and completing all remaining tasks

---

### Testing & Final Adjustments

Before submission, we focused on:

- Bug fixing and stability improvements
- Verifying all core features worked as expected
- Testing user flows:
    - Registration and login
    - Event creation
    - Booking and cancellation
    - Reviews and ratings
    - Search and filtering

All major functionality was tested to ensure the application runs without crashes and meets the assignment requirements.

---

### Outcome

By the end of Sprint 3, the application was complete and met all core requirements:

- Full event management system
- Booking and capacity handling
- Search and filtering functionality
- Review and rating system
- Clear and consistent console interface

The project was finalized with documentation, testing, and preparation for submission.