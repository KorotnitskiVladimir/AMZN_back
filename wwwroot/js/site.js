// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assdo
document.addEventListener("submit", e => {
    const form = e.target;
    if(form.id === "user-registration-form") {
        e.preventDefault();
        fetch("/Admin/Register", {
            method: 'POST',
            body: new FormData(form)
        }).then(r => r.json())
            .then(j => {
                if(j.success) {
                    alert("User registered successfully!");
                    form.reset();
                    window.location.reload();
                }
                else {
                    alert(j.message);
                }
            });
    }
    if(form.id === "auth-modal-form") {
        e.preventDefault();
        const email = form.querySelector("[name='AuthEmail']").value;
        const password = form.querySelector("[name='AuthPassword']").value;
        const credentials = btoa(email + ':' + password);
        fetch("/Admin/Login", {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + credentials
            }
        }).then(r => r.json())
            .then(j => {
                if(j.status === 200) {
                    alert("Logged in successfully!");
                    window.location.reload();
                }
                else {
                    alert(j.message);
                }
            });
    }
    if(form.id === "add-category-form") {
        e.preventDefault();
        fetch("/Admin/AddCategory", {
            method: 'POST',
            body: new FormData(form)
        }).then(r => r.json())
            .then(j => {
                if(j.success) {
                    alert("Category added successfully!");
                    form.reset();
                    window.location.reload();
                }
                else {
                    alert(j.message);
                }
            });
    }
})
