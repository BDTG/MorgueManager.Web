import { initializeApp } from "https://www.gstatic.com/firebasejs/10.11.0/firebase-app.js";
import { getAuth, signInWithPopup, GoogleAuthProvider } from "https://www.gstatic.com/firebasejs/10.11.0/firebase-auth.js";

let app;
let auth;
let provider;

// Hàm khởi tạo Firebase được gọi động từ Blazor WASM bằng cấu hình từ appsettings.json
window.initializeFirebase = (config) => {
    try {
        app = initializeApp(config);
        auth = getAuth(app);
        provider = new GoogleAuthProvider();
    } catch (error) {
        console.error("Lỗi khởi tạo Firebase:", error);
    }
};

// Hàm được thiết lập ở global scope (window) để Blazor WASM có thể gọi thông qua JSInterop
window.loginWithGoogle = async () => {
    if (!auth || !provider) {
        console.error("Firebase Auth chưa được khởi tạo!");
        return {
            success: false,
            errorMessage: "Firebase Auth chưa được khởi tạo. Vui lòng kiểm tra lại cấu hình."
        };
    }
    try {
        const result = await signInWithPopup(auth, provider);
        const user = result.user;
        const token = await user.getIdToken();
        
        // Trả về dữ liệu cần thiết cho C# xử lý
        return {
            success: true,
            token: token,
            email: user.email,
            displayName: user.displayName,
            photoUrl: user.photoURL
        };
    } catch (error) {
        console.error("Lỗi đăng nhập Google:", error);
        return {
            success: false,
            errorMessage: error.message
        };
    }
};
