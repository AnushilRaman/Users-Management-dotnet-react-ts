import { ReactNode, createContext, useReducer, useCallback, useEffect, act } from "react";
import {
    IAuthContext,
    IAuthContextAction,
    IAuthContextActionTypes,
    IAuthContextState,
    ILoginResponseDto
} from "../types/auth.types";
import { getSession, setSession } from "./auth.utils";
import axiosInstance from "../utils/axiosInstance";
import toast from 'react-hot-toast';
import { useNavigate } from 'react-router-dom';
import {
    LOGIN_URL,
    ME_URL,
    PATH_AFTER_LOGIN,
    PATH_AFTER_LOGOUT,
    PATH_AFTER_REGISTER,
    REGISTER_URL
} from "../utils/globalConfig";

const authReducer = (state: IAuthContextState, action: IAuthContextAction) => {
    if (action.type === IAuthContextActionTypes.LOGIN) {
        return {
            ...state,
            isAuthenticated: true,
            isAuthLoading: false,
            user: action.payload
        }
    }
    if (action.type === IAuthContextActionTypes.LOGOUT) {
        return {
            ...state,
            isAuthenticated: false,
            isAuthLoading: false,
            user: undefined
        }
    }
    return state;
}

const initialAuthState: IAuthContextState = {
    isAuthenticated: false,
    isAuthLoading: false,
    user: undefined
}

export const AuthContext = createContext<IAuthContext | null>(null);