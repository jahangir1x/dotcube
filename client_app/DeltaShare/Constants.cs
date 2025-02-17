﻿namespace DeltaShare
{
    public static class Constants
    {
        // App API routes
        public const string NewClientPath = "/new-client";
        public const string ClientsSyncPath = "/clients-sync";
        public const string NewFileMetadataPath = "/new-files";

        // App API form fields
        public const string UserJsonField = "UserJson";
        public const string AllUsersJsonField = "AllUsersJson";
        public const string UserFilesJsonField = "UserFilesJson";

        // default values
        public const string DefaultUsername = "John Doe";

        // preferences keys
        public const string SettingsShowedKey = "IsSettingsShowed";
        public const string UsernameKey = "Username";
        public const string FullNameKey = "FullName";

        // temporary creator values
        public const string PoolCreatorIpAddress = "192.168.1.103";
        public const string Port = "9898";

        // window size
        public const int WindowWidth = 500;
        public const int WindowHeight = 800;
    }
}
