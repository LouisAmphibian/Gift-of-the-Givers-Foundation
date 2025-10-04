const loginOpenBtn = document.querySelector("#form-open-login"),
    signupOpenBtn = document.querySelector("#form-open-signup"),
    home = document.querySelector(".home"),
    formContainer = document.querySelector(".form_container"),
    formCloseBtn = document.querySelector(".form_close"),
    goToSignupLink = document.querySelector("#go-signup"),
    goToLoginLink = document.querySelector("#go-login")
    pwShowHide = document.querySelectorAll(".pw_hide");
    
pwShowHide.forEach((icon) => {
    icon.addEventListener("click", () => {
        let getPwInput = icon.parentElement.querySelector("input");
        if (getPwInput.type === "password") {
            getPwInput.type = "text";
            icon.classList.replace("uil-eye-slash", "uil-eye");
        } else {
            getPwInput.type = "password";
            icon.classList.replace("uil-eye", "uil-eye-slash"); 
        }
    });
});

// open login form
loginOpenBtn.addEventListener("click", () => {
    home.classList.add("show");
    formContainer.classList.add("show-login");
    formContainer.classList.remove("show-signup");
});

// open signup form
signupOpenBtn.addEventListener("click", () => {
    home.classList.add("show");
    formContainer.classList.add("show-signup");
    formContainer.classList.remove("show-login");
});

// close form
formCloseBtn.addEventListener("click", () => {
    home.classList.remove("show");
    formContainer.classList.remove("show-login", "show-signup");
});

// go to signup (from login form)
goToSignupLink.addEventListener("click", (e) => {
    e.preventDefault();
    formContainer.classList.add("show-signup");
    formContainer.classList.remove("show-login");
});

// go to login (from signup form)
goToLoginLink.addEventListener("click", (e) => {
    e.preventDefault();
    formContainer.classList.add("show-login");
    formContainer.classList.remove("show-signup");
});
