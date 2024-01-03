package demo.cleanslice;

import com.j256.ormlite.jdbc.JdbcConnectionSource;
import com.j256.ormlite.table.TableUtils;

import demo.datalayer.model.Ingredient;
import demo.datalayer.model.Recipe;

public class MigrationApplication {
    public static void main(String[] args) {
        String database = "CleanSlice"; // System.getenv("POSTGRES_DB");
        String username = "CleanSliceUser"; // System.getenv("POSTGRES_USER");
        String password = "Password12@"; // System.getenv("POSTGRES_PASSWORD");

        // Check if the required environment variables are set
        if (database == null || username == null || password == null) {
            System.out.println(
                    "Missing environment variables. Please make sure POSTGRES_DB, POSTGRES_USER, and POSTGRES_PASSWORD are set.");
            return;
        }

        String databaseUrl = String.format("jdbc:postgresql://%s:%s/%s", "localhost", "5432", database);
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
