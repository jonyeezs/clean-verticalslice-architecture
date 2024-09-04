package demo.cleanslice.datalayer;

import java.util.UUID;

import org.springframework.stereotype.Repository;

import java.sql.SQLException;

import com.j256.ormlite.dao.BaseDaoImpl;
import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.support.ConnectionSource;

import demo.cleanslice.datalayer.entities.Recipe;

public interface RecipeDao extends Dao<Recipe, UUID> {

}

@Repository
class RecipeDaoImpl extends BaseDaoImpl<Recipe, UUID> implements RecipeDao {
    public RecipeDaoImpl(ConnectionSource connectionSource) throws SQLException {
        super(connectionSource, Recipe.class);
    }
}
