package demo.cleanslice.usecases.create_recipe;

import java.sql.SQLException;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;

import org.springframework.stereotype.Service;

import demo.cleanslice.datalayer.RecipeDao;

@Service
public class DataAccess {
    private RecipeDao recipeDao;

    public DataAccess(RecipeDao recipeDao) {
        this.recipeDao = recipeDao;
    }

    public RecipeBookDomain retrieve() {
        return new RecipeBookDomain(this::findRecipeByTitle);
    }

    protected List<Recipe> findRecipeByTitle(String title) {
        try {
            return this.recipeDao.queryForEq("title", title)
                    .stream()
                    .map(r -> new Recipe(r.title, null))
                    .toList();
        } catch (SQLException e) {
            return Collections.emptyList();
        }
    }

    public RecipeBookDomain Add(RecipeBookDomain domain) throws SQLException {
        List<demo.cleanslice.datalayer.entities.Recipe> recipes = domain.recipes
                .stream()
                .map(r -> {
                    var recipe = new demo.cleanslice.datalayer.entities.Recipe();
                    recipe.title = r.getTitle();
                    recipe.ingredients = Arrays.asList(r.getIngredients()).stream()
                            .map(i -> new demo.cleanslice.datalayer.entities.Ingredient(i.getName())).toList();
                    return recipe;
                }).toList();

        this.recipeDao.create(recipes);
        return domain;
    }

}
