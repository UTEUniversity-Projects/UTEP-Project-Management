﻿using ProjectManagement.DAOs;
using ProjectManagement.Database;
using ProjectManagement.Process;
using ProjectManagement.Enums;
using ProjectManagement.Utils;
using Microsoft.VisualBasic.ApplicationServices;

namespace ProjectManagement.Models
{
    public class Users
    {

        #region USER ATTRIBUTES

        protected string userId;
        protected string userName;
        protected string fullName;
        protected string password;
        protected string email;
        protected string phoneNumber;
        protected DateTime dateOfBirth;
        protected string citizenCode;
        protected string university;
        protected string faculty;
        protected string workCode;
        protected EUserGender gender;
        protected string avatar;
        protected EUserRole role;
        protected DateTime joinAt;

        #endregion

        #region USER CONTRUCTORS

        public Users()
        {
            userId = string.Empty;
            userName = string.Empty;
            fullName = string.Empty;
            password = string.Empty;
            email = string.Empty;
            phoneNumber = string.Empty;
            dateOfBirth = DateTime.Now;
            citizenCode = string.Empty;
            university = "HCM City University of Technology and Education";
            faculty = "Faculty of Information Technology";
            workCode = string.Empty;
            gender = EUserGender.MALE;
            avatar = "PicAvatarDemoUser";
            role = EUserRole.STUDENT;
            joinAt = DateTime.Now;
        }

        public Users(string userId, string userName, string fullName, string password, string email, string phoneNumber,
            DateTime dateOfBirth, string citizenCode, string university, string faculty, string workCode,
            EUserGender gender, string avatar, EUserRole role, DateTime joinAt)
        {
            this.userId = userId;
            this.userName = userName;
            this.fullName = fullName;
            this.password = password;
            this.email = email;
            this.phoneNumber = phoneNumber;
            this.dateOfBirth = dateOfBirth;
            this.citizenCode = citizenCode;
            this.university = university;
            this.faculty = faculty;
            this.workCode = workCode;
            this.gender = gender;
            this.avatar = avatar;
            this.role = role;
            this.joinAt = joinAt;
        }

        public Users(string userName, string fullName, string password,
            string email, string phoneNumber, DateTime dateOfBirth, string citizenCode,
            string university, string faculty, string workCode, EUserGender gender, string avatar, EUserRole role, DateTime joinAt)
        {
            this.userId = (role == EUserRole.LECTURE) ? ModelUtil.GenerateModelId(EModelClassification.LECTURE)
                                                       : ModelUtil.GenerateModelId(EModelClassification.STUDENT);
            this.userName = userName;
            this.fullName = fullName;
            this.password = password;
            this.email = email;
            this.phoneNumber = phoneNumber;
            this.dateOfBirth = dateOfBirth;
            this.citizenCode = citizenCode;
            this.university = university;
            this.faculty = faculty;
            this.workCode = workCode;
            this.gender = gender;
            this.avatar = avatar;
            this.role = role;
            this.joinAt = joinAt;
        }

        #endregion

        #region USER PROPERTIES

        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        public string FullName
        {
            get { return fullName; }
            set {  fullName = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        public DateTime DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
        }
        public string CitizenCode
        {
            get { return citizenCode; }
            set { citizenCode = value; }
        }
        public string University
        {
            get { return university; }
            set { university = value; }
        }
        public string Faculty
        {
            get { return faculty; }
            set { faculty = value; }
        }
        public string WorkCode
        {
            get { return workCode; }
            set { workCode = value; }
        }
        public EUserGender Gender
        {
            get { return gender; }
            set { gender = value; }
        }
        public string Avatar
        {
            get { return avatar; }
            set { avatar = value; }
        }
        public EUserRole Role
        {
            get { return role; }
            set { role = value; }
        }
        public DateTime JoinAt
        {
            get { return joinAt; }
            set { joinAt = value; }
        }

        #endregion

        #region CHECK INFORMATIONS

        public bool CheckFullName()
        {
            return DAOUtils.CheckIsNotEmpty(this.fullName, "FullName");
        }
        public bool CheckCitizenCode()
        {
            return DAOUtils.CheckIsNotEmpty(this.citizenCode, "CitizenCode")
                    && DAOUtils.CheckNonExist("Users", "citizencode", this.citizenCode);
        }
        public bool CheckBirthday()
        {
            return DAOUtils.CheckIsValidAge(this.dateOfBirth, "DateOfBirth");
        }
        public bool CheckGender()
        {
            return this.gender == EUserGender.MALE || this.gender == EUserGender.FEMALE;
        }
        public bool CheckEmail()
        {
            return DAOUtils.CheckIsNotEmpty(this.email, "Email")
                    && DAOUtils.CheckNonExist("Users", "email", this.email);
        }
        public bool CheckPhoneNumber()
        {
            return DAOUtils.CheckIsNotEmpty(this.phoneNumber, "PhoneNumber") && this.phoneNumber.All(char.IsDigit)
                    && DAOUtils.CheckNonExist("Users", "phoneNumber", this.phoneNumber);
        }
        public bool CheckUserName()
        {
            return DAOUtils.CheckIsNotEmpty(this.userName, "UserName")
                   && DAOUtils.CheckNonExist("Users", "userName", this.userName);
        }
        public bool CheckRole()
        {
            return this.role == EUserRole.LECTURE || this.role == EUserRole.STUDENT;
        }
        public bool CheckWorkCode()
        {
            return DAOUtils.CheckIsNotEmpty(this.workCode, "WorkCode");
        }
        public bool CheckUniversity()
        {
            return DAOUtils.CheckIsNotEmpty(this.university, "University");
        }
        public bool CheckFaculty()
        {
            return DAOUtils.CheckIsNotEmpty(this.faculty, "Faculty");
        }
        public bool CheckPassWord(string confirmPassword)
        {
            return DAOUtils.CheckIsNotEmpty(this.password, "Password") && this.password == confirmPassword;
        }

        #endregion

        #region FUNCTIONS

        public Users Clone()
        {
            return new Users(userId, userName, fullName, password, email, phoneNumber, dateOfBirth,
                citizenCode, university, faculty, workCode, gender, avatar, role, joinAt);
        }

        #endregion

    }
}
