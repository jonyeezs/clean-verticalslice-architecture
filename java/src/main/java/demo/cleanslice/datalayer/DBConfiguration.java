package demo.cleanslice.datalayer;

import java.sql.SQLException;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import com.j256.ormlite.jdbc.JdbcConnectionSource;
import com.j256.ormlite.support.ConnectionSource;

@Configuration
public class DBConfiguration {

    @Value("${database.connectionstring}")
    private String databaseUrl;

    @Value("${database.username}")
    private String databaseUsername;

    @Value("${database.password}")
    private String databasePassword;

    @Bean
    public ConnectionSource connectionSource() throws SQLException {
        return new JdbcConnectionSource(databaseUrl, databaseUsername, databasePassword);
    }
}
