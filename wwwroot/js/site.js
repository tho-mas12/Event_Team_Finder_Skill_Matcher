// JavaScript for Event Finder & Skill Matcher Application

document.addEventListener("DOMContentLoaded", function () {

    // 1. Initialize Deadline Countdown Timers
    const countdownElements = document.querySelectorAll('.countdown-timer');
    countdownElements.forEach(el => {
        const deadlineStr = el.getAttribute('data-deadline');
        if (deadlineStr) {
            const deadline = new Date(deadlineStr).getTime();

            const timerInterval = setInterval(function () {
                const now = new Date().getTime();
                const distance = deadline - now;

                if (distance < 0) {
                    clearInterval(timerInterval);
                    el.innerHTML = "<span class='badge bg-danger'><i class='bi bi-clock-history me-1'></i> Registration Closed</span>";
                } else {
                    const days = Math.floor(distance / (1000 * 60 * 60 * 24));
                    const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
                    const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
                    const seconds = Math.floor((distance % (1000 * 60)) / 1000);

                    el.innerHTML = `<span class="badge bg-warning text-dark px-3 py-2 fs-6 shadow-sm"><i class="bi bi-hourglass-split me-1"></i> ${days}d ${hours}h ${minutes}m ${seconds}s left</span>`;
                }
            }, 1000);
        }
    });

    // 2. Dynamic Program Builder for Admin Create Event Page
    const addProgramBtn = document.getElementById('add-program-btn');
    const programContainer = document.getElementById('programs-container');

    if (addProgramBtn && programContainer) {
        let programCount = programContainer.querySelectorAll('.program-item').length;

        addProgramBtn.addEventListener('click', function () {
            const index = programCount;
            const cardHtml = `
                <div class="card border-0 shadow-sm rounded-4 mb-4 program-item p-4 bg-light position-relative">
                    <button type="button" class="btn-close position-absolute top-0 end-0 m-3 remove-program-btn" aria-label="Close"></button>
                    <h5 class="fw-bold text-primary mb-3"><i class="bi bi-card-heading me-2"></i>Program #${index + 1}</h5>
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label fw-semibold">Program Name</label>
                            <input type="text" name="Programs[${index}].ProgramName" class="form-control" placeholder="e.g. Bug Bounty / UI Design" required />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label fw-semibold">Max Participants</label>
                            <input type="number" name="Programs[${index}].MaxParticipants" class="form-control" value="30" min="1" required />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label fw-semibold">Team Size</label>
                            <input type="number" name="Programs[${index}].TeamSize" class="form-control" value="1" min="1" required />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label fw-semibold">Event Date & Time</label>
                            <input type="datetime-local" name="Programs[${index}].EventDate" class="form-control" required />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label fw-semibold">Registration Deadline Date & Time</label>
                            <input type="datetime-local" name="Programs[${index}].Deadline" class="form-control" required />
                        </div>
                        <div class="col-md-12">
                            <label class="form-label fw-semibold">Program Description</label>
                            <input type="text" name="Programs[${index}].Description" class="form-control" placeholder="Brief summary of program" />
                        </div>
                        <div class="col-md-12">
                            <label class="form-label fw-semibold">Rules & Guidelines</label>
                            <textarea name="Programs[${index}].Rules" class="form-control" rows="3" placeholder="1. Rule one&#10;2. Rule two" required></textarea>
                        </div>
                    </div>
                </div>
            `;
            programContainer.insertAdjacentHTML('beforeend', cardHtml);
            programCount++;
        });

        // Delegate remove program button click
        programContainer.addEventListener('click', function (e) {
            if (e.target.classList.contains('remove-program-btn')) {
                const card = e.target.closest('.program-item');
                if (programContainer.querySelectorAll('.program-item').length > 1) {
                    card.remove();
                } else {
                    alert("At least one event program is required.");
                }
            }
        });
    }
});
