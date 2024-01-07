package demo.cleanslice;

import com.j256.ormlite.jdbc.JdbcConnectionSource;
import com.j256.ormlite.table.TableUtils;

import demo.cleanslice.datalayer.entities.Ingredient;
import demo.cleanslice.datalayer.entities.Recipe;

public class MigrationApplication {
    public static void main(String[] args) {
        String DBHost = System.getenv("DB_HOST") != null ? System.getenv("DB_HOST") : "localhost";
        String DBPort = System.getenv("DB_PORT") != null ? System.getenv("DB_PORT") : "5432";
        String database = System.getenv("DB_NAME") != null ? System.getenv("DB_DB") : "CleanSlice";
        String username = System.getenv("DB_USER") != null ? System.getenv("DB_USER") : "CleanSliceUser";
        String password = System.getenv("DB_PASSWORD") != null ? System.getenv("DB_PASSWORD")
                : "Password12@";

        // Check if the required environment variables are set
        if (database == null || username == null || password == null) {
            System.out.println(
                    "Missing environment variables. Please make sure DB_HOST, DB_PORT, DB_NAME, DB_USER and DB_PASSWORD are set.");
            return;
        }

        String databaseUrl = String.format("jdbc:postgresql://%s:%s/%s", DBHost, DBPort, database);
        System.out.println(databaseUrl);
        try {
            // Create a connection source to the database
            JdbcConnectionSource connectionSource = new JdbcConnectionSource(databaseUrl, username, password);

            // Create the tables based on the ORMLite models
            TableUtils.createTableIfNotExists(connectionSource, Ingredient.class);
            TableUtils.createTableIfNotExists(connectionSource, Recipe.class);
            // Add more model classes if needed

            // Close the connection source
            connectionSource.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
